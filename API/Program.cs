using API.Services;
using Application;
using Application.Common.Interfaces;
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

app.UseStaticFiles(); // Permite servir archivos estáticos, como imágenes y documentos

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<API.Hubs.KitchenHub>("/kitchenHub"); // Mapea el hub de SignalR

app.Run();
