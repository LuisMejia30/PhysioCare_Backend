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

            var rawConn = Environment.GetEnvironmentVariable("DATABASE_URL")
                       ?? builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? builder.Configuration["ConnectionStrings__DefaultConnection"]
                       ?? builder.Configuration["DefaultConnection"];

            var finalConn = ParsePostgresConnectionString(rawConn);

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(finalConn));

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

            var jwtKey = builder.Configuration["Jwt:Key"]
                      ?? builder.Configuration["Jwt__Key"]
                      ?? Environment.GetEnvironmentVariable("Jwt__Key")
                      ?? Environment.GetEnvironmentVariable("Jwt:Key");

            if (string.IsNullOrWhiteSpace(jwtKey) || jwtKey.Length < 32)
            {
                jwtKey = "PhysioCare_Super_Secret_Key_2026_Minimum_32_Bytes_Long!";
            }

            var jwtIssuer = builder.Configuration["Jwt:Issuer"]
                         ?? builder.Configuration["Jwt__Issuer"]
                         ?? Environment.GetEnvironmentVariable("Jwt__Issuer")
                         ?? Environment.GetEnvironmentVariable("Jwt:Issuer")
                         ?? "PhysioCare";

            var jwtAudience = builder.Configuration["Jwt:Audience"]
                           ?? builder.Configuration["Jwt__Audience"]
                           ?? Environment.GetEnvironmentVariable("Jwt__Audience")
                           ?? Environment.GetEnvironmentVariable("Jwt:Audience")
                           ?? "PhysioCareUsers";

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

        public static string ParsePostgresConnectionString(string? connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                return "Host=localhost;Database=PhysioCareDB;Username=postgres;Password=postgres";
            }

            // Si viene en formato URL tipo postgres://usuario:password@host:puerto/bd (Render / Neon)
            if (connectionString.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase) ||
                connectionString.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
            {
                var uri = new Uri(connectionString);
                var userInfo = uri.UserInfo.Split(':');
                var username = userInfo[0];
                var password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : "";
                var host = uri.Host;
                var port = uri.Port > 0 ? uri.Port : 5432;
                var database = uri.AbsolutePath.TrimStart('/');

                return $"Host={host};Port={port};Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true;";
            }

            return connectionString;
        }
    }
}