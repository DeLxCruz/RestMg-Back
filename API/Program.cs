using API.Services;
using Application;
using Application.Common.Interfaces;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

// -- EXTENSIONES DE REGISTRO --
builder.Services
    .AddApplicationServices()
    .AddInfrastructureServices(builder.Configuration);

var serviceAccountJsonPath = builder.Configuration["Firebase:AdminSdkPath"]
    ?? throw new InvalidOperationException("Firebase:AdminSdkPath no está configurado.");

FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile(serviceAccountJsonPath),
});

// -- Servicios propios de la capa de API --
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// -- Hubs --
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("CorsPolicy"); // Aplica la política de CORS definida

app.UseStaticFiles(); // Permite servir archivos estáticos, como imágenes y documentos

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<API.Hubs.KitchenHub>("/kitchenHub"); // Mapea el hub de SignalR

app.Run();
