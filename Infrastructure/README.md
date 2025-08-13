# ⚙️ Infrastructure Layer

> **Capa de Infraestructura - Conectando el Mundo Exterior**

## 📋 Descripción

La capa de **Infrastructure** implementa todos los servicios externos y tecnologías específicas que el sistema necesita para funcionar. Esta capa actúa como un **adaptador** entre el dominio y el mundo exterior, implementando las interfaces definidas en la Application Layer.

## 🎯 Principios de Diseño

### Dependency Inversion Principle
- Implementa interfaces definidas en capas superiores
- Permite cambiar tecnologías sin afectar el negocio
- Facilita testing con mocks y fakes

### Adapter Pattern
- Adapta servicios externos al dominio interno
- Encapsula complejidades técnicas
- Proporciona abstracciones limpias

### Repository Pattern
- Encapsula lógica de acceso a datos
- Proporciona una interfaz orientada a colecciones
- Facilita testing y cambio de proveedores de datos

## 📁 Estructura del Proyecto

```
Infrastructure/
├── DependencyInjection.cs       # Registro de servicios
├── Database/                    # Configuración de base de datos
│   ├── AppDbContext.cs
│   └── EntityConfigurations/
│       ├── RestaurantConfiguration.cs
│       ├── TableConfiguration.cs
│       ├── MenuItemConfiguration.cs
│       ├── CategoryConfiguration.cs
│       ├── OrderConfiguration.cs
│       ├── OrderItemConfiguration.cs
│       └── UserConfiguration.cs
├── Auth/                        # Servicios de autenticación
│   ├── JwtService.cs
│   ├── PasswordService.cs
│   └── CurrentUserService.cs
├── Services/                    # Servicios externos
│   ├── EmailService.cs
│   ├── FirebaseStorageService.cs
│   └── NotificationService.cs
└── Migrations/                  # Migraciones de EF Core
    ├── 20241201000001_Initial.cs
    └── ...
```

## 🗄️ Acceso a Datos

### ¿Qué es Entity Framework Core?
Entity Framework Core es un **ORM (Object-Relational Mapper)**. Imagine que tiene un asistente personal que habla perfectamente dos idiomas: C# (el lenguaje de programación) y SQL (el lenguaje de las bases de datos).

**¿Cómo funciona este "traductor"?**

1. **Nosotros escribimos en C#**: `context.Restaurants.Where(r => r.Name == "Mi Restaurante")`
2. **Entity Framework lo traduce a SQL**: `SELECT * FROM Restaurants WHERE Name = 'Mi Restaurante'`
3. **La base de datos ejecuta el SQL** y nos devuelve los resultados
4. **Entity Framework convierte los resultados** de vuelta a objetos C# que podemos usar

**¿Por qué es mejor que escribir SQL directamente?**
- **Más seguro**: Previene ataques de inyección SQL automáticamente
- **Más fácil**: No necesitas memorizar sintaxis SQL complicada
- **Portable**: Si cambias de SQL Server a PostgreSQL, el código C# no cambia
- **Menos errores**: El compilador te avisa si cometes errores antes de ejecutar

### AppDbContext (El Centro de Comunicación con la Base de Datos)
**Responsabilidad**: Es como el "directorio telefónico" de nuestra base de datos. Sabe dónde encontrar cada tabla y cómo comunicarse con ella.

```csharp
public class AppDbContext : DbContext
{
    public DbSet<Restaurant> Restaurants { get; set; }    // Tabla de restaurantes
    public DbSet<Table> Tables { get; set; }             // Tabla de mesas
    public DbSet<MenuItem> MenuItems { get; set; }       // Tabla de elementos del menú
    public DbSet<Category> Categories { get; set; }      // Tabla de categorías
    public DbSet<Order> Orders { get; set; }             // Tabla de pedidos
    public DbSet<OrderItem> OrderItems { get; set; }     // Tabla de elementos de pedido
    public DbSet<User> Users { get; set; }               // Tabla de usuarios
}
```

**¿Qué significa cada DbSet?**
Cada `DbSet<>` representa una tabla en la base de datos. Es como decir "aquí tengo una colección de restaurantes, una colección de mesas, etc."

**Características:**
- Configuración fluida con Entity Configurations
- Soporte para migraciones automáticas
- Optimizado para consultas específicas del dominio
- Connection string configurable por ambiente

### Entity Configurations (Las Reglas de Construcción de las Tablas)

**¿Qué son las Entity Configurations?**
Son como "planos de construcción" que le dicen a Entity Framework exactamente cómo debe crear cada tabla en la base de datos. Es como cuando un arquitecto especifica que una habitación debe tener ciertas dimensiones, tipos de material, etc.

#### RestaurantConfiguration (Configuración de la Tabla Restaurantes)
```csharp
public void Configure(EntityTypeBuilder<Restaurant> builder)
{
    builder.HasKey(r => r.Id);                    // "Id" es la llave primaria
    builder.Property(r => r.Name)
        .IsRequired()                             // El nombre es obligatorio
        .HasMaxLength(100);                       // Máximo 100 caracteres
    
    builder.HasIndex(r => r.Name)
        .IsUnique();                              // No puede haber dos restaurantes con el mismo nombre
        
    // Relación: Un restaurante puede tener muchas mesas
    builder.HasMany(r => r.Tables)               // Un restaurante tiene muchas mesas
        .WithOne(t => t.Restaurant)              // Cada mesa pertenece a un restaurante
        .HasForeignKey(t => t.RestaurantId)      // La conexión se hace por RestaurantId
        .OnDelete(DeleteBehavior.Cascade);       // Si elimino el restaurante, se eliminan sus mesas
}
```

**Traducido a lenguaje humano:**
- "Crea una tabla llamada Restaurants"
- "La columna Id es la clave principal (identificador único)"
- "La columna Name es obligatoria y no puede tener más de 100 caracteres"
- "No puede haber dos restaurantes con el mismo nombre"
- "Si elimino un restaurante, elimina también todas sus mesas automáticamente"

#### OrderConfiguration (Configuración de la Tabla Pedidos)
```csharp
public void Configure(EntityTypeBuilder<Order> builder)
{
    builder.HasKey(o => o.Id);                   // Id como llave primaria
    
    builder.Property(o => o.Code)
        .IsRequired()                            // El código es obligatorio
        .HasMaxLength(10);                       // Máximo 10 caracteres
        
    builder.Property(o => o.Total)
        .HasColumnType("decimal(18,2)");         // Precio con 18 dígitos, 2 decimales
        
    builder.HasIndex(o => new { o.RestaurantId, o.Code })
        .IsUnique();                             // No puede haber dos pedidos con el mismo código en el mismo restaurante
}
```

**Traducido a lenguaje humano:**
- "Crea una tabla llamada Orders"
- "El código del pedido es obligatorio y corto (máximo 10 caracteres)"
- "El total se guarda como decimal con precisión para dinero"
- "Cada pedido debe tener un código único dentro de su restaurante (pero restaurantes diferentes pueden tener códigos iguales)"

### Patrones de Configuración

#### Unique Constraints
- Nombres de restaurante únicos globalmente
- Códigos de mesa únicos por restaurante  
- Códigos de orden únicos por restaurante
- Emails de usuario únicos globalmente

#### Cascade Behaviors
- Restaurant → Tables: CASCADE (eliminar restaurant elimina mesas)
- Restaurant → Categories: CASCADE
- Category → MenuItems: RESTRICT (no eliminar categoría con items)
- Order → OrderItems: CASCADE

#### Data Types
- Precios: `decimal(18,2)` para precisión monetaria
- Fechas: `DateTime` con UTC
- Strings: Longitudes máximas definidas
- Enums: Almacenados como integers

## 🔐 Servicios de Autenticación

### JwtService
**Responsabilidad**: Generación y validación de tokens JWT

```csharp
public interface IJwtService
{
    string GenerateToken(User user);
    bool ValidateToken(string token);
    ClaimsPrincipal GetPrincipalFromToken(string token);
}
```

**Características:**
- Tokens firmados con HS256
- Claims personalizados (UserId, RestaurantId, Role)
- Expiración configurable
- Validación de signature y expiración

### PasswordService
**Responsabilidad**: Hash y verificación de contraseñas

```csharp
public interface IPasswordService
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hashedPassword);
}
```

**Implementación:**
- BCrypt con salt factor 12
- Resistente a ataques de fuerza bruta
- Hash irreversible pero verificable

### CurrentUserService
**Responsabilidad**: Obtener información del usuario actual

```csharp
public interface ICurrentUserService
{
    int? UserId { get; }
    int? RestaurantId { get; }
    string? Role { get; }
    bool IsAuthenticated { get; }
}
```

**Fuente de datos:**
- Claims del JWT en HttpContext
- Inyectado en Application Services
- Usado para autorización y auditoría

## 🔥 Servicios Externos

### FirebaseStorageService (Guardado de Imágenes)

**¿Qué es Firebase Storage?**
Es un servicio de Google (como Google Drive, pero para aplicaciones) donde guardamos las fotos de los platos del menú y logos de restaurantes.

**¿Por qué es una buena opción?**
- **Confiabilidad**: Google garantiza que las imágenes siempre estén disponibles (99.9% del tiempo)
- **Velocidad**: Las imágenes se cargan muy rápido desde cualquier parte del mundo
- **Económico**: Solo pagamos por lo que usamos, y para restaurantes pequeños es prácticamente gratis
- **Seguridad**: Google se encarga de proteger las imágenes contra hackers
- **Escalabilidad**: Si el restaurante crece y sube más fotos, no hay problema de espacio

**¿Qué hace exactamente nuestro servicio?**
```csharp
public interface IFileStorageService
{
    Task<string> SaveFileAsync(string fileName, byte[] fileBytes);
    Task<bool> DeleteFileAsync(string fileName);
    Task<byte[]> GetFileAsync(string fileName);
}
```

**Traducido a lenguaje humano:**
- `SaveFileAsync`: Recibe una imagen y la guarda en Firebase, nos devuelve la "dirección web" donde quedó guardada
- `DeleteFileAsync`: Elimina una imagen cuando ya no la necesitamos  
- `GetFileAsync`: Trae una imagen específica cuando la necesitamos

**¿Qué nos devuelve?**
Una URL (dirección web) como: `https://firebasestorage.googleapis.com/v0/b/restmg-app.appspot.com/o/menu-items%2Fhamburguesa.jpg`

**Configuración necesaria:**
```json
{
  "Firebase": {
    "Type": "service_account",
    "ProjectId": "restaurant-management-xxxx",
    "StorageBucket": "restaurant-management-xxxx.appspot.com"
  }
}
```

**Ventajas para nuestro sistema:**
- Las imágenes se organizan automáticamente en carpetas (logos/, menu-items/)
- Si nuestro servidor falla, las imágenes siguen disponibles
- Los clientes ven las fotos rápidamente sin importar desde dónde se conecten
- No nos preocupamos por el espacio de almacenamiento en nuestro servidor

## 📊 Migraciones de Base de Datos (Cómo se Crea la BD desde Código)

### ¿Qué son las Migraciones?
Las migraciones son como "planos de construcción" que le dicen a la base de datos cómo debe crearse o modificarse. En lugar de crear tablas manualmente, escribimos código C# y Entity Framework se encarga de convertirlo en comandos SQL.

### ¿Cómo funciona el proceso?
1. **Escribimos el modelo en C#**: Definimos nuestras clases (Restaurant, Order, etc.)
2. **Generamos la migración**: `dotnet ef migrations add NombreCambio`
3. **Entity Framework crea el "plano"**: Analiza nuestras clases y genera código SQL automáticamente
4. **Aplicamos los cambios**: `dotnet ef database update`
5. **La base de datos se actualiza**: Se crean o modifican las tablas según nuestro código C#

### Estrategia de Migraciones

#### Desarrollo
```bash
dotnet ef migrations add MigrationName --project Infrastructure --startup-project API
dotnet ef database update --project Infrastructure --startup-project API
```

#### Producción
```csharp
// Program.cs
if (app.Environment.IsProduction())
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Database.Migrate();
    }
}
```

### Historial de Migraciones

#### Initial (20241201000001)
- Creación de todas las tablas base
- Índices únicos y relaciones
- Datos semilla para desarrollo

#### AddOrderTracking (20241205000001)
- Campos adicionales en Order
- Estados de orden expandidos
- Índices de performance

#### AddBrandingFeatures (20241210000001)
- Campos de branding en Restaurant
- Configuración de colores y logos
- URLs personalizadas

## 🔧 Dependency Injection

### DependencyInjection.cs
```csharp
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        // Database
        services.AddDbContext<AppDbContext>(options =>
        {
            var connectionString = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production" 
                ? configuration.GetConnectionString("DeployConnection")
                : configuration.GetConnectionString("DefaultConnection");
            options.UseSqlServer(connectionString);
        });

        // Authentication
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        // External Services
        services.AddScoped<IFileStorageService, FirebaseStorageService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<INotificationService, NotificationService>();

        return services;
    }
}
```

### Configuración por Ambiente

#### Development
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=RestaurantMgDB;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

#### Production
```json
{
  "ConnectionStrings": {
    "DeployConnection": "Server=SQL-SERVER-URL;Database=RestaurantMgDB;User Id=username;Password=password;TrustServerCertificate=true;"
  }
}
```

## 🔍 Logging y Monitoreo

### Database Logging
```csharp
options.UseSqlServer(connectionString)
    .LogTo(Console.WriteLine, LogLevel.Information)
    .EnableSensitiveDataLogging(isDevelopment);
```

### Service Logging
```csharp
public class FirebaseStorageService : IFileStorageService
{
    private readonly ILogger<FirebaseStorageService> _logger;
    
    public async Task<string> SaveFileAsync(string fileName, byte[] fileBytes)
    {
        _logger.LogInformation("Uploading file: {FileName}", fileName);
        
        try
        {
            // Upload logic
            _logger.LogInformation("File uploaded successfully: {Url}", url);
            return url;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload file: {FileName}", fileName);
            throw;
        }
    }
}
```

## 🧪 Pruebas y Monitoreo del Proyecto

### Estrategia de Pruebas Implementada
En este proyecto académico se realizaron **pruebas manuales exhaustivas** para validar todas las funcionalidades del sistema. Se priorizó demostrar la arquitectura y el funcionamiento correcto sobre la automatización de pruebas.

**Pruebas manuales realizadas:**
- **Pruebas de Servicios**: Verificación de la integración con Firebase Storage, autenticación JWT, y servicios de base de datos
- **Pruebas de Configuración**: Validación de Entity Configurations y migraciones de base de datos
- **Pruebas de Seguridad**: Verificación del hash de contraseñas, validación de tokens JWT, y control de acceso
- **Pruebas de Conexión**: Testing de la conexión a base de datos en diferentes ambientes (desarrollo y producción)

**Casos de prueba validados:**
- Creación y recuperación de registros en todas las entidades
- Funcionamiento correcto de las relaciones entre tablas
- Aplicación correcta de constraints y validaciones
- Manejo de errores en servicios externos (Firebase, base de datos)
- Transiciones de estado válidas e inválidas

### Despliegue en Producción
**Plataforma utilizada: MonsterASP.NET**

**¿Qué es MonsterASP.NET?**
Es una plataforma de hosting especializada en aplicaciones .NET que permite gestionar tanto la aplicación como la base de datos SQL Server desde un solo panel de control.

**Proceso de despliegue realizado:**
1. **Compilación**: Se compiló la aplicación usando `dotnet publish -c Release`
2. **Configuración**: Se actualizó `appsettings.json` con la cadena de conexión de producción
3. **Base de datos**: Se creó la base de datos SQL Server en MonsterASP.NET
4. **Migraciones**: Se ejecutó `dotnet ef database update` apuntando a la conexión de producción
5. **Despliegue**: Se subieron los archivos compilados al hosting
6. **Configuración de IIS**: Se configuró el archivo `web.config` para el manejo de la aplicación ASP.NET Core

**Configuración de producción aplicada:**
```json
{
  "ConnectionStrings": {
    "DeployConnection": "Server=sql-server-url;Database=RestaurantMgDB;User Id=username;Password=password;TrustServerCertificate=true;"
  }
}
```

**Comando de migración utilizado en producción:**
```bash
dotnet ef database update --connection "Server=sql-server-url;Database=RestaurantMgDB;User Id=username;Password=password;TrustServerCertificate=true;"
```

### Monitoreo Básico Implementado
**Logging básico configurado:**
- Logs de aplicación a través del sistema de logging de ASP.NET Core
- Registro de errores críticos en servicios externos
- Monitoreo básico de conexiones de base de datos

**En futuras iteraciones o producción real se implementaría:**
- **Testing automatizado avanzado**: Unit tests, integration tests, load testing
- **Logging profesional**: Structured logging con Serilog, centralización de logs
- **Monitoreo avanzado**: Application Insights, health checks automáticos, métricas de performance
- **CI/CD**: Pipeline automatizado de despliegue con Azure DevOps o GitHub Actions

## 📈 Configuración Implementada

### Configuración de Base de Datos
La configuración actual utiliza Entity Framework Core con SQL Server de forma básica:

```csharp
services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});
```

### Optimizaciones Aplicadas
- **Índices únicos** definidos en las Entity Configurations
- **Relaciones con Cascade Delete** configuradas apropiadamente  
- **Tipos de datos específicos** para campos monetarios (decimal(18,2))
- **Consultas con Include** para cargar datos relacionados eficientemente

## 🔒 Seguridad Implementada

### Configuración de Connection Strings
- Uso de variables de entorno para diferenciar desarrollo y producción
- Connection strings separadas para cada ambiente

### Seguridad JWT Implementada  
- Generación de tokens con algoritmo HS256
- Claims específicos para UserId, RestaurantId y Role
- Validación de tokens en cada request autenticado

### Seguridad de Base de Datos
- Uso de Entity Framework Core para prevención de SQL injection
- Conexiones encriptadas con `TrustServerCertificate=true`

## 🚀 Despliegue Realizado

### Proceso de Migración de Base de Datos
El proyecto utiliza Entity Framework Core migrations para gestionar la estructura de la base de datos:

**Comandos utilizados en desarrollo:**
```bash
dotnet ef migrations add MigrationName --project Infrastructure --startup-project API
dotnet ef database update --project Infrastructure --startup-project API
```

**Comando utilizado en producción (MonsterASP.NET):**
```bash
dotnet ef database update --connection "Server=sql-server-url;Database=RestaurantMgDB;User Id=username;Password=password;TrustServerCertificate=true;" --project Infrastructure --startup-project API
```

---

## 🔗 Enlaces Relacionados

- [Domain Layer](../Domain/README.md)
- [Application Layer](../Application/README.md)  
- [API Layer](../API/README.md)
- [Documentación Principal](../README.md)
