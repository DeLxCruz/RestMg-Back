using Application.Common.Interfaces;
using Google.Apis.Auth.OAuth2;
using Infrastructure.Auth;
using Infrastructure.Database;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Configuración de Base de Datos
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Registrar la interfaz para que pueda ser inyectada en la capa de Application
            services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<AppDbContext>());

            // Servicios de Infraestructura
            services.AddScoped<IPasswordService, PasswordService>();
            services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

            // Configuración de CORS
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });

            // Configuración de Autenticación con JWT
            var jwtSettings = configuration.GetSection("JwtSettings");
            var secret = jwtSettings["Secret"]!;

            services.AddAuthentication(defaultScheme: JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),

                    RoleClaimType = ClaimTypes.Role
                });

            // Registrar el servicio de almacenamiento de archivos
            services.AddSingleton(sp =>
            {
                var serviceAccountJsonPath = configuration["Firebase:AdminSdkPath"]
                    ?? throw new InvalidOperationException("Firebase:AdminSdkPath no está configurado.");
                
                return GoogleCredential.FromFile(serviceAccountJsonPath);
            });
            services.AddScoped<IFileStorageService, FirebaseStorageService>();

            // Registrar el generador de códigos QR
            services.AddSingleton<IQrCodeGenerator, QrCodeGenerator>();

            #if DEBUG
            services.AddHttpClient("default")
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                });
            #endif

            return services;
        }
    }
}