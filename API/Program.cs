using API.Hubs;
using API.Services;
using Application;
using Application.Common.Interfaces;
using Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

// -- EXTENSIONES DE REGISTRO --
builder.Services
    .AddApplicationServices()
    .AddInfrastructureServices(builder.Configuration);

// -- Servicios propios de la capa de API --
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<INotificationsHub, NotificationsHubService>();

// -- Hubs --
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
// Habilitar Swagger siempre para el proyecto de grado
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "RestMg API v1");
    c.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();

// Servir archivos estáticos
app.UseStaticFiles();

// Enrutamiento
app.UseRouting();

// CORS
app.UseCors("CorsPolicy");

// Autenticación - Identifica quién es el usuario.
app.UseAuthentication();

// Autorización - Verifica si el usuario tiene permiso.
app.UseAuthorization();

// Habilitar WebSockets
app.UseWebSockets();

app.MapControllers();

app.MapHub<KitchenHub>("/kitchenHub");
app.MapHub<NotificationsHub>("/notificationsHub");

app.Run();