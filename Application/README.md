# 🔧 Application Layer

> **Capa de Aplicación - Los Casos de Uso del Negocio**

## 📋 Descripción

La capa de **Application** actúa como coordinador central del sistema, conteniendo todos los casos de uso del negocio (crear pedidos, gestionar menús, etc.) y orquestando su ejecución. Esta capa implementa la lógica de aplicación siguiendo el patrón CQRS (Command Query Responsibility Segregation) con MediatR como mediador.

## 🏗️ Arquitectura CQRS y MediatR

### Implementación de CQRS
**CQRS (Command Query Responsibility Segregation)** separa las responsabilidades entre operaciones de escritura (Commands) y operaciones de lectura (Queries):

- **Commands (Comandos)**: Ejecutan acciones que modifican el estado del sistema (crear, actualizar, eliminar)
- **Queries (Consultas)**: Recuperan información sin modificar el estado del sistema

**Beneficios de esta separación:**
- **Claridad de responsabilidades**: Cada operación tiene un propósito específico
- **Mantenibilidad**: Los cambios en comandos no afectan las consultas y viceversa
- **Optimización independiente**: Commands y Queries pueden ser optimizados por separado
- **Escalabilidad**: Permite diferentes estrategias de scaling para lectura y escritura

### Patrón Mediator con MediatR
**MediatR** implementa el patrón Mediator, actuando como intermediario entre los controllers y los handlers:

**Flujo de procesamiento:**
1. El controller recibe una petición HTTP
2. Crea un Command/Query object con los datos necesarios
3. Envía el objeto a través de MediatR usando `mediator.Send()`
4. MediatR localiza automáticamente el Handler correspondiente
5. Ejecuta el Handler y retorna la respuesta

**Ventajas del patrón:**
- **Bajo acoplamiento**: Los controllers no conocen directamente los handlers
- **Inyección de dependencias automática**: MediatR resuelve automáticamente las dependencias
- **Pipeline behaviors**: Permite agregar funcionalidades transversales (validación, logging, caching)

### Estructura de Features
```
Features/
├── FeatureName/
│   ├── Commands/
│   │   └── CommandName/
│   │       ├── Command.cs
│   │       ├── CommandHandler.cs
│   │       └── CommandValidator.cs (opcional)
│   ├── Queries/
│   │   └── QueryName/
│   │       ├── Query.cs
│   │       ├── QueryHandler.cs
│   │       └── QueryResult.cs
│   └── Notifications/
│       └── NotificationName.cs
```

## 📁 Estructura del Proyecto

### `/Common`
Contiene interfaces y utilidades compartidas:

- **`/Interfaces`**: Contratos de servicios externos
  - `IApplicationDbContext.cs` - Contexto de base de datos
  - `ICurrentUserService.cs` - Usuario actual
  - `IFileStorageService.cs` - Almacenamiento de archivos
  - `IJwtTokenGenerator.cs` - Generación de tokens JWT
  - `IPasswordService.cs` - Hashing de contraseñas
  - `IQrCodeGenerator.cs` - Generación de códigos QR

- **`/Notifications`**: Eventos del dominio
  - Notificaciones para comunicación entre bounded contexts

### `/Features`
Casos de uso organizados por características de negocio:

## 🔍 Features Implementados

### 🔐 Auth - Autenticación
**Casos de Uso:**
- `Login` - Autenticación de usuarios
- `RefreshToken` - Renovación de tokens JWT

**Responsabilidades:**
- Validación de credenciales
- Generación de tokens JWT
- Gestión de refresh tokens

### 🎨 Branding - Marca del Restaurante
**Casos de Uso:**
- `UploadLogo` - Subida de logos
- `UpdateRestaurantLogo` - Asociar logo al restaurante
- `GetRestaurantLogo` - Obtener logo del restaurante

**Responsabilidades:**
- Gestión de assets visuales
- Integración con Firebase Storage
- Manejo de archivos multimedia

### 🍽️ Menu - Gestión de Menús
**Casos de Uso:**
- `GetFullMenu` - Obtener menú completo
- `GetFullMenuBySubdomain` - Menú por subdominio público
- `GetMenuItemDetail` - Detalle de elemento del menú

**Responsabilidades:**
- Presentación de menús públicos
- Filtrado por disponibilidad
- Optimización de consultas

### 📦 MenuItems - Gestión de Productos
**Casos de Uso:**
- `CreateMenuItem` - Crear nuevo elemento
- `UpdateMenuItem` - Actualizar elemento existente
- `DeleteMenuItem` - Eliminar elemento

**Responsabilidades:**
- CRUD de elementos del menú
- Validación de reglas de negocio
- Gestión de imágenes de productos

### 📱 Orders - Gestión de Pedidos
**Casos de Uso:**
- `CreateOrder` - Crear nuevo pedido
- `GetOrderByCode` - Buscar pedido por código

**Responsabilidades:**
- Procesamiento de pedidos
- Validación de disponibilidad
- Generación de códigos únicos
- Notificaciones en tiempo real

### 👨‍🍳 Kitchen - Módulo de Cocina
**Casos de Uso:**
- `GetKitchenOrders` - Órdenes activas de cocina
- `StartOrder` - Iniciar preparación
- `MarkOrderReady` - Marcar como listo
- `ConfirmOrderPayment` - Confirmar pago
- `GetKitchenHistory` - Historial de órdenes

**Responsabilidades:**
- Workflow de preparación de órdenes
- Estados de pedidos (Pending → InPreparation → Ready → Completed)
- Métricas de rendimiento
- Notificaciones push a dispositivos

### 🪑 Tables - Gestión de Mesas
**Casos de Uso:**
- `CreateTable` - Crear nueva mesa
- `UpdateTable` - Actualizar mesa
- `GetTables` - Listar mesas
- `GetTableQrCode` - Generar QR de mesa

**Responsabilidades:**
- Administración de mesas
- Generación de códigos QR únicos
- Control de estados de mesa

### 👥 Users - Gestión de Usuarios
**Casos de Uso:**
- `CreateUser` - Crear nuevo usuario
- `UpdateUser` - Actualizar usuario
- `DeleteUser` - Eliminar usuario
- `GetUsers` - Listar usuarios

**Responsabilidades:**
- CRUD de usuarios del sistema
- Gestión de roles y permisos
- Hashing seguro de contraseñas

### 🏪 Restaurants - Gestión de Restaurantes
**Casos de Uso:**
- `Onboard` - Registro inicial de restaurante
- `UpdateMyRestaurant` - Actualizar información
- `GetMyRestaurant` - Obtener información actual

**Responsabilidades:**
- Proceso de onboarding
- Configuración de subdominios
- Gestión de configuraciones específicas

### 📊 Reports - Reportes y Analytics
**Casos de Uso:**
- `GetOrdersReport` - Reporte de ventas
- `GetBestsellersReport` - Productos más vendidos

**Responsabilidades:**
- Generación de reportes
- Análisis de datos de venta
- Métricas de performance

### 📈 Dashboard - Panel de Control
**Casos de Uso:**
- `GetDashboardSummary` - Resumen ejecutivo
- `GetTopDishesToday` - Platos destacados del día

**Responsabilidades:**
- Métricas en tiempo real
- KPIs de negocio
- Agregación de datos

## 🛠️ Patrones Utilizados

### 1. Command Pattern
```csharp
public record CreateMenuItemCommand(
    Guid CategoryId,
    string Name,
    string Description,
    decimal Price,
    string? ImageUrl
) : IRequest<Guid>;
```

### 2. Query Pattern
```csharp
public record GetFullMenuQuery(Guid RestaurantId) : IRequest<MenuDto>;
```

### 3. Handler Pattern
```csharp
public class CreateMenuItemCommandHandler : IRequestHandler<CreateMenuItemCommand, Guid>
{
    // Implementación del caso de uso
}
```

### 4. Notification Pattern
```csharp
public record OrderStatusChangedNotification(
    Guid OrderId,
    OrderStatus NewStatus
) : INotification;
```

## ⚡ Características Técnicas

### MediatR Pipeline
- **Request/Response**: Commands y Queries
- **Notifications**: Eventos del dominio
- **Behaviors**: Cross-cutting concerns (logging, validation, caching)

### Validación
- FluentValidation para reglas complejas
- Validaciones automáticas en el pipeline de MediatR

### Mapeo
- AutoMapper para transformación de DTOs
- Mapeo automático entre entidades y modelos de vista

### Manejo de Errores
- Exceptions personalizadas por dominio
- Middleware de manejo global de errores

## 🔌 Dependencias

### Inyección de Dependencias
```csharp
// DependencyInjection.cs
public static IServiceCollection AddApplicationServices(
    this IServiceCollection services)
{
    services.AddMediatR(typeof(DependencyInjection).Assembly);
    services.AddAutoMapper(typeof(DependencyInjection).Assembly);
    // ... otras configuraciones
}
```

### Interfaces Requeridas
- `IApplicationDbContext` - Acceso a datos
- `ICurrentUserService` - Usuario autenticado
- `IFileStorageService` - Almacenamiento de archivos
- `IJwtTokenGenerator` - Generación de tokens
- `IPasswordService` - Hashing de contraseñas

## 🔄 Flujo de Datos

### Command Flow
```
Controller → Command → CommandHandler → Domain → Database
                                    ↓
                                Notifications → SignalR Hub
```

### Query Flow
```
Controller → Query → QueryHandler → Database → DTO → Response
```

## 🧪 Testing y Logging (No Implementados en Este Proyecto Académico)

### ¿Por qué no incluimos Testing de Application Handlers?
En este proyecto académico nos enfocamos en demostrar la lógica de negocio y la arquitectura. En un entorno de producción real incluiríamos:

**Testing que se implementaría:**
- **Handler Tests**: Pruebas de cada comando y query por separado
- **Business Logic Tests**: Verificación de que las reglas de negocio se cumplan correctamente
- **Integration Tests**: Pruebas que verifican el flujo completo incluyendo base de datos

**Ejemplo de lo que se probaría:**
- ¿Se crea correctamente un pedido cuando todos los datos son válidos?
- ¿Se rechaza un pedido cuando un producto no está disponible?
- ¿Se envía la notificación correcta cuando se crea un nuevo pedido?

### ¿Por qué no incluimos Logging de Aplicación?
El logging en la Application Layer permite rastrear cada acción de negocio. En producción incluiríamos:

**Logging que se implementaría:**
- **Command/Query Logging**: Registrar cada acción que se ejecuta
- **Business Rule Logging**: Registrar cuándo se aplican las reglas de negocio
- **Performance Logging**: Medir cuánto tiempo toma cada caso de uso
- **Error Logging**: Registrar errores de lógica de negocio con contexto detallado

### ¿Por qué no incluimos Telemetría de Aplicación?
La telemetría nos permite entender cómo se usa el sistema. En producción incluiríamos:

**Telemetría que se implementaría:**
- **Usage Metrics**: Estadísticas de qué features se usan más
- **Business Metrics**: KPIs como número de pedidos por hora, productos más vendidos
- **Performance Metrics**: Tiempos de respuesta de cada caso de uso
- **Error Rate Metrics**: Frecuencia de errores por feature

## 📋 Mejores Prácticas

### 1. Single Responsibility
Cada handler tiene una única responsabilidad específica.

### 2. Separation of Concerns
Commands y Queries separados claramente.

### 3. Fail Fast
Validaciones tempranas en el pipeline.

### 4. Idempotencia
Commands diseñados para ser idempotentes cuando es posible.

### 5. Event-Driven
Uso de notifications para desacoplamiento.

### 6. Rich Domain Model
Lógica de negocio encapsulada en el dominio.

---

## 🔗 Enlaces Relacionados

- [Domain Layer](../Domain/README.md)
- [Infrastructure Layer](../Infrastructure/README.md)
- [API Layer](../API/README.md)
- [Documentación Principal](../README.md)
