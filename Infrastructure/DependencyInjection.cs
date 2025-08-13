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
            // Configuración de Base de Datos - seleccionar según ambiente
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var connectionString = environment == "Development"
                ? configuration.GetConnectionString("DefaultConnection")
                : configuration.GetConnectionString("DeployConnection");

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
                    var clientUrl = configuration["ClientAppSettings:ClientUrl"]
                        ?? throw new InvalidOperationException("ClientAppSettings:ClientUrl no está configurado.");

                    builder.WithOrigins(clientUrl)
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           .AllowCredentials();
                });
            });

            // Configuración de Autenticación con JWT
            var jwtSettings = configuration.GetSection("JwtSettings");
            var secret = jwtSettings["Secret"]!;

            services.AddAuthentication(defaultScheme: JwtBearerDefaults.AuthenticationScheme)
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
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                        RoleClaimType = ClaimTypes.Role
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) &&
                                (path.StartsWithSegments("/kitchenHub")))
                            {
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

            // Registrar el servicio de almacenamiento de archivos
            services.AddSingleton(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();

                var serviceAccountJsonPath = config["Firebase:AdminSdkPath"];
                if (!string.IsNullOrEmpty(serviceAccountJsonPath) && File.Exists(serviceAccountJsonPath))
                {
                    return GoogleCredential.FromFile(serviceAccountJsonPath);
                }

                var firebaseConfigJson = config["Firebase:Config"];
                if (!string.IsNullOrEmpty(firebaseConfigJson))
                {
                    return GoogleCredential.FromJson(firebaseConfigJson);
                }

                throw new InvalidOperationException("Las credenciales de Firebase no están configuradas. Se requiere 'Firebase:AdminSdkPath' (para desarrollo) o 'Firebase:Config' (para producción).");
            });

            // El servicio de almacenamiento ahora recibirá la credencial correcta
            // según el entorno en el que se esté ejecutando.
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