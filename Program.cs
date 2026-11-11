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

            // CAMBIO: Política estricta de CORS apuntando directamente a tu dominio de Vercel
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowVercel",
                    policy => policy.WithOrigins("https://physiocare-frontend-chi.vercel.app")
                                    .AllowAnyMethod()
                                    .AllowAnyHeader());
            });

            var jwtKey = builder.Configuration["Jwt:Key"] ?? "MiClaveSuperSecretaPhysioCare123!";
            var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "PhysioCare";
            var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "PhysioCareUsers";

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

            // CAMBIO: Aplicar la política exacta que acabamos de crear
            app.UseCors("AllowVercel");

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            
            // IMPORTANTE: El orden aquí debe mantenerse intacto (Cors -> Authentication -> Authorization)
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}