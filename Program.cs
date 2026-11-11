using Microsoft.EntityFrameworkCore;
using physiocare_backend.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace physiocare_backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Habilitar compatibilidad de fechas/timestamps para PostgreSQL (Neon)
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Configuración de CORS para Vercel y desarrollo local
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowVercel",
                    policy => policy.WithOrigins(
                                        "https://physiocare-frontend-chi.vercel.app",
                                        "http://localhost:5173",
                                        "http://localhost:3000"
                                    )
                                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                                    .AllowAnyMethod()
                                    .AllowAnyHeader());
            });

            var jwtKey = builder.Configuration["Jwt:Key"];
            if (string.IsNullOrWhiteSpace(jwtKey) || jwtKey.Length < 32)
            {
                jwtKey = "PhysioCare_Super_Secret_Key_2026_Minimum_32_Bytes_Long!";
            }

            var jwtIssuer = builder.Configuration["Jwt:Issuer"];
            if (string.IsNullOrWhiteSpace(jwtIssuer)) jwtIssuer = "PhysioCare";

            var jwtAudience = builder.Configuration["Jwt:Audience"];
            if (string.IsNullOrWhiteSpace(jwtAudience)) jwtAudience = "PhysioCareUsers";

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                };
            });

            builder.Services.AddAuthorization();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Auto-migración al arrancar para garantizar que todas las tablas existan en PostgreSQL
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<ApplicationDbContext>();
                    context.Database.Migrate();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Ocurrió un error al aplicar las migraciones a PostgreSQL.");
                }
            }

            // Aplicar CORS como primer middleware para garantizar headers en todas las respuestas
            app.UseCors("AllowVercel");

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Orden estricto de seguridad: Cors -> Authentication -> Authorization
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}