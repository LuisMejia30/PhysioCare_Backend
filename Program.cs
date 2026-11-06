using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using physiocare_backend.Data;

namespace physiocare_backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // 0. Habilitar compatibilidad de fechas/timestamps sin zona horaria para Npgsql (PostgreSQL)
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            var builder = WebApplication.CreateBuilder(args);

            // 1. Configurar la conexión a PostgreSQL (Neon.tech)
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            // 2. Configuración de Autenticación JWT
            var jwtSettings = builder.Configuration.GetSection("Jwt");
            var keyString = jwtSettings["Key"] ?? "PhysioCare_Super_Secret_Key_2026_Minimum_32_Bytes_Long!";
            var key = Encoding.UTF8.GetBytes(keyString);

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings["Issuer"],
                        ValidAudience = jwtSettings["Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(key)
                    };
                });

            // 3. Configurar política de CORS flexible para el Frontend (Vercel / Localhost)
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowReactApp", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            // 4. Configurar Swagger con soporte para Bearer Token
            builder.Services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Ingresa 'Bearer' [espacio] y luego tu token. Ejemplo: 'Bearer eyJhbGci...'"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            var app = builder.Build();

            // 5. Middleware de CORS al inicio de la tubería HTTP (evita bloqueos 401/CORS en React)
            app.UseCors("AllowReactApp");

            // 6. Habilitar Swagger tanto en desarrollo como en producción para pruebas remotas
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "PhysioCare API v1");
                c.RoutePrefix = "swagger";
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}