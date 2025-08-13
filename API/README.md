# 🌐 API Layer

> **Capa de Presentación - La Puerta de Entrada al Sistema**

## 📋 Descripción

La capa **API** es como la "recepción" del sistema. Es el lugar donde llegan todas las peticiones desde las aplicaciones (web, móvil) y las dirige al lugar correcto dentro del sistema. Funciona como el mesero en un restaurante: recibe el pedido del cliente, lo lleva a la cocina, y trae de vuelta la respuesta.

## 🎯 Principios de Diseño

### REST API Design (Diseño de API REST)
**¿Qué es REST?** Es un conjunto de reglas para que las aplicaciones se comuniquen de manera ordenada y predecible.

- **Stateless (Sin Estado)**: Cada petición contiene toda la información necesaria, como cuando se llama a un taxi y se da la dirección completa cada vez
- **Resource-Based (Basado en Recursos)**: Las URLs representan "cosas" del sistema (restaurantes, menús, pedidos), no acciones
- **HTTP Methods (Métodos HTTP)**: Se usan verbos estándar:
  - GET = "Dame información" (como ver el menú)
  - POST = "Crea algo nuevo" (como hacer un pedido)
  - PUT = "Actualiza algo existente" (como cambiar un plato)
  - DELETE = "Elimina algo" (como borrar una mesa)

### API-First Development (Desarrollo API Primero)
- Se diseña primero cómo se van a comunicar las aplicaciones
- Se documenta todo automáticamente con Swagger (como un manual de instrucciones)
- Si se necesitan cambios, se hacen de forma controlada sin romper aplicaciones existentes

### Clean Architecture Principles (Principios de Arquitectura Limpia)
- Los controladores solo reciben peticiones y las pasan al lugar correcto
- No contienen lógica de negocio (eso está en otras capas)
- Traducen entre lo que entienden las aplicaciones externas y lo que entiende el sistema interno

## 🔄 SignalR - Comunicación en Tiempo Real

### Implementación Real de SignalR

**¿Qué hace SignalR en este proyecto?**
SignalR permite que la aplicación de cocina reciba notificaciones en tiempo real cuando suceden eventos importantes, sin necesidad de recargar la página o hacer polling.

### KitchenHub - El Centro de Comunicación

**Código real implementado:**
```csharp
[Authorize]
public class KitchenHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        // Obtiene el RestaurantId del token JWT del usuario autenticado
        var restaurantId = Context.User?.FindFirstValue("restaurantId");

        if (!string.IsNullOrEmpty(restaurantId))
        {
            // Une esta conexión al grupo de su restaurante
            await Groups.AddToGroupAsync(Context.ConnectionId, $"restaurant-{restaurantId}");
        }

        await base.OnConnectedAsync();
    }
}
```

**¿Cómo funciona paso a paso?**
1. **Conexión inicial**: Cuando un usuario (cocina/admin) abre la aplicación, se conecta automáticamente al hub
2. **Autenticación**: SignalR verifica el token JWT del usuario 
3. **Agrupación**: Se une automáticamente al grupo de su restaurante usando el ID del token
4. **Listo**: Ahora puede recibir notificaciones específicas de su restaurante

### Notification Handlers - Los Mensajeros

El sistema usa MediatR para publicar eventos y SignalR para enviarlos en tiempo real:

#### 1. Nueva Orden Recibida
```csharp
public class NewOrderReceivedSignalRHandler : INotificationHandler<NewOrderReceivedNotification>
{
    public Task Handle(NewOrderReceivedNotification notification, CancellationToken ct)
    {
        var groupName = $"restaurant-{notification.RestaurantId}";
        // Envía mensaje "NewOrder" solo al grupo de ese restaurante
        return _hubContext.Clients.Group(groupName)
            .SendAsync("NewOrder", notification, ct);
    }
}
```

**¿Cuándo se dispara?** 
- Cuando un cliente confirma el pago de su pedido
- En `ConfirmOrderPaymentCommandHandler` se publica `NewOrderReceivedNotification`
- Este handler lo captura y lo envía a la cocina vía SignalR

#### 2. Cambio de Estado de Orden
```csharp
public class OrderStatusChangedSignalRHandler : INotificationHandler<OrderStatusChangedNotification>
{
    public Task Handle(OrderStatusChangedNotification notification, CancellationToken ct)
    {
        var groupName = $"restaurant-{notification.RestaurantId}";
        return _hubContext.Clients.Group(groupName)
            .SendAsync("OrderStatusUpdated", notification.OrderId, notification.NewStatus, ct);
    }
}
```

**¿Cuándo se dispara?**
- Cuando cocina marca una orden como "En preparación" (`StartOrderCommand`)
- Cuando cocina marca una orden como "Lista" (`MarkOrderReadyCommand`) 
- Actualiza en tiempo real el estado en todas las pantallas conectadas

#### 3. Cambio de Estado de Mesa
```csharp
public class TableStateChangedSignalRHandler : INotificationHandler<TableStateChangedNotification>
{
    public Task Handle(TableStateChangedNotification notification, CancellationToken cancellationToken)
    {
        var groupName = $"restaurant-{notification.RestaurantId}";
        return _hubContext.Clients.Group(groupName)
            .SendAsync("TableStateUpdated", notification.TableId, notification.NewState, cancellationToken);
    }
}
```

#### 4. Disponibilidad de MenuItem
```csharp
public class MenuItemAvailabilitySignalRHandler : INotificationHandler<MenuItemAvailabilityNotification>
{
    public Task Handle(MenuItemAvailabilityNotification notification, CancellationToken cancellationToken)
    {
        var groupName = $"restaurant-{notification.RestaurantId}";
        return _hubContext.Clients.Group(groupName)
            .SendAsync("MenuItemAvailabilityUpdated", notification.MenuItemId, notification.IsAvailable, cancellationToken);
    }
}
```

### Flujo Completo de una Orden

**Ejemplo real de cómo funciona:**

1. **Cliente hace pedido → Paga**
   ```csharp
   // En ConfirmOrderPaymentCommandHandler
   await publisher.Publish(new NewOrderReceivedNotification(...));
   ```

2. **Cocina recibe notificación instantánea**
   - El `NewOrderReceivedSignalRHandler` envía mensaje `"NewOrder"` via SignalR
   - La aplicación de cocina actualiza su lista sin recargar

3. **Cocina inicia preparación**
   ```csharp
   // En StartOrderCommandHandler  
   order.Status = OrderStatus.InPreparation;
   await publisher.Publish(new OrderStatusChangedNotification(...));
   ```

4. **Todas las pantallas se actualizan**
   - El `OrderStatusChangedSignalRHandler` envía `"OrderStatusUpdated"`
   - Panel de admin y cocina ven el cambio al instante

5. **Cocina marca como lista**
   ```csharp
   // En MarkOrderReadyCommandHandler
   order.Status = OrderStatus.Ready;
   await publisher.Publish(new OrderStatusChangedNotification(...));
   ```

6. **Meseros reciben notificación**
   - Todas las pantallas ven que el pedido está listo para entregar

### Mensajes SignalR Implementados

**Para el cliente JavaScript, estos son los eventos que se pueden escuchar:**

```javascript
connection.on("NewOrder", (orderData) => {
    // Nueva orden llegó a cocina
    console.log("Nueva orden:", orderData);
});

connection.on("OrderStatusUpdated", (orderId, newStatus) => {
    // Estado de orden cambió  
    console.log(`Orden ${orderId} ahora está: ${newStatus}`);
});

connection.on("TableStateUpdated", (tableId, newState) => {
    // Estado de mesa cambió
    console.log(`Mesa ${tableId} ahora está: ${newState}`);
});

connection.on("MenuItemAvailabilityUpdated", (itemId, isAvailable) => {
    // Disponibilidad de item cambió
    console.log(`Item ${itemId} disponible: ${isAvailable}`);
});
```

### Configuración en Program.cs

```csharp
// Agregar SignalR
builder.Services.AddSignalR();

// Configurar el hub endpoint
app.MapHub<KitchenHub>("/hubs/kitchen");
```

**URL de conexión:** `https://tu-api.com/hubs/kitchen`

### Seguridad Implementada

- **Autenticación requerida**: `[Authorize]` en el hub
- **Grupos por restaurante**: Solo recibes notificaciones de tu restaurante
- **JWT validation**: El token se valida automáticamente
- **Claims-based grouping**: Se usa el `restaurantId` del token para agrupar

### 📊 Diagrama de Flujo: SignalR en Tiempo Real

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   App Cocina    │    │   App Admin     │    │   App Cliente   │
│   (Frontend)    │    │   (Frontend)    │    │   (Frontend)    │
└─────────┬───────┘    └─────────┬───────┘    └─────────┬───────┘
          │                      │                      │
          │ 1. Conecta con JWT   │ 1. Conecta con JWT   │ 1. Hace pedido
          ▼                      ▼                      ▼
┌─────────────────────────────────────────────────────────────────┐
│                    KitchenHub (SignalR)                        │
│  • Recibe conexiones autenticadas                              │
│  • Une usuarios a grupo: "restaurant-{RestaurantId}"          │
│  • Valida permisos automáticamente                            │
└─────────────────────┬───────────────────────────────────────────┘
                      │
                      │ 2. Cliente paga pedido
                      ▼
┌─────────────────────────────────────────────────────────────────┐
│              ConfirmOrderPaymentCommandHandler                  │
│  • Cambia estado: AwaitingPayment → Pending                   │
│  • Publica: NewOrderReceivedNotification                      │
└─────────────────────┬───────────────────────────────────────────┘
                      │
                      │ 3. MediatR enruta notification
                      ▼
┌─────────────────────────────────────────────────────────────────┐
│            NewOrderReceivedSignalRHandler                       │
│  • Recibe notification de MediatR                             │
│  • Envía "NewOrder" via SignalR al grupo del restaurante      │
└─────────────────────┬───────────────────────────────────────────┘
                      │
                      │ 4. SignalR envía mensaje
                      ▼
┌─────────────────────────────────────────────────────────────────┐
│                 Clientes Conectados                            │
│  ┌─────────────────┐    ┌─────────────────┐                   │
│  │   App Cocina    │    │   App Admin     │                   │
│  │                 │    │                 │                   │
│  │ • Recibe        │    │ • Recibe        │                   │
│  │   "NewOrder"    │    │   "NewOrder"    │                   │
│  │ • Actualiza     │    │ • Actualiza     │                   │
│  │   pantalla      │    │   dashboard     │                   │
│  │   sin recargar  │    │   sin recargar  │                   │
│  └─────────────────┘    └─────────────────┘                   │
└─────────────────────────────────────────────────────────────────┘

Resultado: 🎯 NOTIFICACIÓN EN TIEMPO REAL SIN POLLING
```

### 🔄 Flujo Completo de Estados con SignalR

```
Estado Inicial: AwaitingPayment
           ↓
    [Cliente paga] ───→ SignalR: "NewOrder" ───→ 📱 Cocina ve pedido
           ↓
      Pending ←─────────────────────────────────────┘
           ↓
    [Cocina inicia] ───→ SignalR: "OrderStatusUpdated" ───→ 📱 Admin ve cambio
           ↓
   InPreparation ←─────────────────────────────────────────┘
           ↓
   [Cocina termina] ───→ SignalR: "OrderStatusUpdated" ───→ 📱 Meseros ven pedido listo
           ↓
      Ready ←─────────────────────────────────────────────┘
           ↓
   [Mesero entrega]
           ↓
     Delivered
```

## 📁 Estructura del Proyecto

```
API/
├── Program.cs                   # Punto de entrada y configuración
├── appsettings.json            # Configuración base
├── appsettings.Development.json # Configuración desarrollo
├── Controllers/                # Controladores REST
│   ├── AuthController.cs
│   ├── RestaurantsController.cs
│   ├── TablesController.cs
│   ├── MenuController.cs
│   ├── MenuManagementController.cs
│   ├── OrdersController.cs
│   ├── KitchenController.cs
│   ├── BrandingController.cs
│   ├── UsersController.cs
│   └── ReportsController.cs
├── DTOs/                       # Data Transfer Objects
│   ├── LoginRequest.cs
│   ├── CreateUserRequest.cs
│   ├── CreateTableRequest.cs
│   ├── OnboardRestaurantRequest.cs
│   ├── UpdateMenuItemRequest.cs
│   ├── UpdateCategoryRequest.cs
│   ├── UpdateRestaurantRequest.cs
│   ├── UpdateTableRequest.cs
│   └── UpdateUserRequest.cs
├── Hubs/                       # SignalR Hubs
│   └── KitchenHub.cs
├── NotificationHandlers/       # Handlers para eventos
│   ├── NewOrderReceivedSignalRHandler.cs
│   ├── OrderStatusChangedSignalRHandler.cs
│   ├── MenuItemAvailabilitySignalRHandler.cs
│   └── TableStateChangedSignalRHandler.cs
└── Services/
    └── CurrentUserService.cs
```

## 🎮 Controladores REST

### 🔐 AuthController
**Responsabilidad**: Autenticación y autorización de usuarios

```csharp
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(RefreshResponse), 200)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshRequest request)
}
```

**Endpoints:**
- `POST /api/auth/login` - Autenticar usuario
- `POST /api/auth/refresh` - Refrescar token JWT

### 🏪 RestaurantsController
**Responsabilidad**: Gestión de restaurantes

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RestaurantsController : ControllerBase
{
    [HttpPost("onboard")]
    [ProducesResponseType(typeof(OnboardResponse), 201)]
    public async Task<IActionResult> OnboardRestaurant([FromBody] OnboardRestaurantRequest request)
    
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(RestaurantResponse), 200)]
    public async Task<IActionResult> GetRestaurant(int id)
    
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(RestaurantResponse), 200)]
    public async Task<IActionResult> UpdateRestaurant(int id, [FromBody] UpdateRestaurantRequest request)
}
```

**Endpoints:**
- `POST /api/restaurants/onboard` - Crear nuevo restaurante
- `GET /api/restaurants/{id}` - Obtener restaurante por ID
- `PUT /api/restaurants/{id}` - Actualizar restaurante

### 🪑 TablesController
**Responsabilidad**: Gestión de mesas del restaurante

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TablesController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(List<TableResponse>), 200)]
    public async Task<IActionResult> GetTables()
    
    [HttpPost]
    [ProducesResponseType(typeof(TableResponse), 201)]
    public async Task<IActionResult> CreateTable([FromBody] CreateTableRequest request)
    
    [HttpGet("{id}/qr")]
    [HttpHead("{id}/qr")]
    [ProducesResponseType(typeof(byte[]), 200)]
    public async Task<IActionResult> GetTableQrCode(int id)
}
```

**Endpoints:**
- `GET /api/tables` - Listar mesas del restaurante
- `POST /api/tables` - Crear nueva mesa
- `GET/HEAD /api/tables/{id}/qr` - Generar código QR de mesa

### 🍽️ MenuController
**Responsabilidad**: Visualización del menú para clientes

```csharp
[ApiController]
[Route("api/[controller]")]
public class MenuController : ControllerBase
{
    [HttpGet("restaurant/{restaurantId}")]
    [ProducesResponseType(typeof(MenuResponse), 200)]
    public async Task<IActionResult> GetMenuByRestaurant(int restaurantId)
    
    [HttpGet("restaurant/{restaurantId}/categories")]
    [ProducesResponseType(typeof(List<CategoryResponse>), 200)]
    public async Task<IActionResult> GetCategoriesByRestaurant(int restaurantId)
}
```

**Endpoints:**
- `GET /api/menu/restaurant/{restaurantId}` - Menú completo para cliente
- `GET /api/menu/restaurant/{restaurantId}/categories` - Categorías del menú

### ⚙️ MenuManagementController
**Responsabilidad**: Gestión administrativa del menú

```csharp
[ApiController]
[Route("api/menu-management")]
[Authorize]
public class MenuManagementController : ControllerBase
{
    [HttpGet("categories")]
    [ProducesResponseType(typeof(List<CategoryResponse>), 200)]
    public async Task<IActionResult> GetCategories()
    
    [HttpPost("categories")]
    [ProducesResponseType(typeof(CategoryResponse), 201)]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequest request)
    
    [HttpPost("menu-items")]
    [ProducesResponseType(typeof(MenuItemResponse), 201)]
    public async Task<IActionResult> CreateMenuItem([FromBody] CreateMenuItemRequest request)
    
    [HttpPut("menu-items/{id}/availability")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> UpdateMenuItemAvailability(int id, [FromBody] UpdateAvailabilityRequest request)
}
```

**Endpoints:**
- `GET /api/menu-management/categories` - Gestionar categorías
- `POST /api/menu-management/categories` - Crear categoría
- `POST /api/menu-management/menu-items` - Crear elemento del menú
- `PUT /api/menu-management/menu-items/{id}/availability` - Cambiar disponibilidad

### 📱 OrdersController
**Responsabilidad**: Gestión de órdenes de clientes

```csharp
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(OrderResponse), 201)]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    
    [HttpGet("{code}/status")]
    [ProducesResponseType(typeof(OrderStatusResponse), 200)]
    public async Task<IActionResult> GetOrderStatus(string code)
    
    [HttpGet("restaurant/{restaurantId}")]
    [Authorize]
    [ProducesResponseType(typeof(List<OrderResponse>), 200)]
    public async Task<IActionResult> GetRestaurantOrders(int restaurantId)
}
```

**Endpoints:**
- `POST /api/orders` - Crear nueva orden (público)
- `GET /api/orders/{code}/status` - Estado de orden (público)
- `GET /api/orders/restaurant/{restaurantId}` - Órdenes del restaurante

### 📋 Diagrama de Flujo: POST /api/orders (Crear Pedido)

**Ejemplo: Cliente hace un pedido desde su teléfono**

```
┌─────────────────┐
│   📱 Cliente    │
│  escanea QR de  │  1. POST /api/orders
│  mesa y hace    │ ────────────────────┐
│  pedido         │                     ▼
└─────────────────┘           ┌─────────────────┐
                              │  OrdersController│
                              │  • Recibe JSON   │
                              │  • Valida datos  │
                              └─────────┬───────┘
                                        │ 2. Crea CreateOrderCommand
                                        ▼
                              ┌─────────────────┐
                              │    MediatR      │
                              │  • Enruta a     │
                              │    Handler      │
                              └─────────┬───────┘
                                        │ 3. Send(command)
                                        ▼
                              ┌─────────────────┐
                              │CreateOrderCommand│
                              │Handler          │
                              │                 │
                              │ 4. Validaciones:│
                              │  ✓ Mesa existe  │
                              │  ✓ Platos exist │
                              │  ✓ Disponibles  │
                              └─────────┬───────┘
                                        │ 5. Si OK, crea Order
                                        ▼
                              ┌─────────────────┐
                              │  📊 Database    │
                              │  • INSERT Order │
                              │  • INSERT Items │
                              │  • UPDATE Table │
                              │    (Occupied)   │
                              └─────────┬───────┘
                                        │ 6. Genera código único
                                        ▼
                              ┌─────────────────┐
                              │    Response     │
                              │  {              │
                              │   "orderCode":  │
                              │   "ORD-A7B2",   │
                              │   "status":     │
                              │   "AwaitingPay" │
                              │  }              │
                              └─────────┬───────┘
                                        │ 7. HTTP 201 Created
                                        ▼
┌─────────────────┐           ┌─────────────────┐
│   📱 Cliente    │  ◄────────│     API         │
│  recibe código  │           │   Response      │
│  ORD-A7B2 para  │           │                 │
│  seguimiento    │           │                 │
└─────────────────┘           └─────────────────┘

✅ RESULTADO: Cliente tiene código para seguir su pedido
```

### 🔄 Arquitectura de Capas en Acción

**¿Cómo viajan los datos por las capas?**

```
📱 Cliente (JSON) → 🌐 API Layer → 🎯 Application Layer → 🏛️ Domain Layer → 📊 Database

PASO A PASO:

1. API Layer (Controllers):
   ┌─────────────────────────┐
   │ OrdersController        │
   │ • Recibe HTTP Request   │
   │ • Valida DTO           │
   │ • Convierte a Command  │
   └─────────┬───────────────┘
             │
2. Application Layer (Use Cases):
   ┌─────────▼───────────────┐
   │ CreateOrderCommandHandler│
   │ • Lógica de negocio     │
   │ • Validaciones         │
   │ • Coordina operaciones │
   └─────────┬───────────────┘
             │
3. Domain Layer (Entities):
   ┌─────────▼───────────────┐
   │ Order, OrderItem       │
   │ • Reglas de dominio    │
   │ • Cálculos (totales)   │
   │ • Validaciones básicas │
   └─────────┬───────────────┘
             │
4. Infrastructure Layer (Data):
   ┌─────────▼───────────────┐
   │ AppDbContext           │
   │ • Entity Framework     │
   │ • SQL Server           │
   │ • Persistencia        │
   └─────────────────────────┘

VENTAJAS DE ESTA ARQUITECTURA:
✓ Separación clara de responsabilidades
✓ Fácil testing (cada capa independiente)
✓ Mantenible (cambios no afectan otras capas)
✓ Escalable (se puede cambiar BD sin afectar lógica)
```

### 👨‍🍳 KitchenController
**Responsabilidad**: Operaciones de cocina

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Kitchen")]
public class KitchenController : ControllerBase
{
    [HttpGet("orders")]
    [ProducesResponseType(typeof(List<KitchenOrderResponse>), 200)]
    public async Task<IActionResult> GetKitchenOrders()
    
    [HttpPut("orders/{id}/status")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusRequest request)
}
```

**Endpoints:**
- `GET /api/kitchen/orders` - Órdenes pendientes en cocina
- `PUT /api/kitchen/orders/{id}/status` - Actualizar estado de orden

## 📊 DTOs (Data Transfer Objects)

### Requests (Entrada)

#### LoginRequest
```csharp
public class LoginRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    
    [Required]
    [MinLength(6)]
    public string Password { get; set; }
}
```

#### CreateOrderRequest
```csharp
public class CreateOrderRequest
{
    [Required]
    public int TableId { get; set; }
    
    [MaxLength(100)]
    public string? CustomerName { get; set; }
    
    [MaxLength(500)]
    public string? Notes { get; set; }
    
    [Required]
    [MinLength(1)]
    public List<CreateOrderItemRequest> Items { get; set; } = new();
}
```

#### OnboardRestaurantRequest
```csharp
public class OnboardRestaurantRequest
{
    [Required]
    [MaxLength(100)]
    public string RestaurantName { get; set; }
    
    [Required]
    [MaxLength(7)]
    [RegularExpression(@"^#[0-9A-Fa-f]{6}$")]
    public string BrandingColor { get; set; }
    
    [Required]
    [EmailAddress]
    public string AdminEmail { get; set; }
    
    [Required]
    [MinLength(6)]
    public string AdminPassword { get; set; }
}
```

### Responses (Salida)

#### MenuResponse
```csharp
public class MenuResponse
{
    public int RestaurantId { get; set; }
    public string RestaurantName { get; set; }
    public string BrandingColor { get; set; }
    public string? LogoUrl { get; set; }
    public List<CategoryWithItemsResponse> Categories { get; set; } = new();
}
```

#### OrderResponse
```csharp
public class OrderResponse
{
    public int Id { get; set; }
    public string Code { get; set; }
    public OrderStatus Status { get; set; }
    public decimal Total { get; set; }
    public string? CustomerName { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public List<OrderItemResponse> Items { get; set; } = new();
}
```

## 🔄 SignalR Hubs

### KitchenHub
**Responsabilidad**: Comunicación en tiempo real con cocina

```csharp
[Authorize]
public class KitchenHub : Hub
{
    public async Task JoinRestaurantGroup(int restaurantId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"Restaurant_{restaurantId}");
    }
    
    public async Task LeaveRestaurantGroup(int restaurantId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Restaurant_{restaurantId}");
    }
}
```

**Eventos enviados a clientes:**
- `NewOrderReceived`: Nueva orden llegó a cocina
- `OrderStatusChanged`: Estado de orden cambió
- `MenuItemAvailabilityChanged`: Disponibilidad de item cambió
- `TableStateChanged`: Estado de mesa cambió

### Notification Handlers

#### NewOrderReceivedSignalRHandler
```csharp
public class NewOrderReceivedSignalRHandler : INotificationHandler<NewOrderReceived>
{
    public async Task Handle(NewOrderReceived notification, CancellationToken cancellationToken)
    {
        await _hubContext.Clients
            .Group($"Restaurant_{notification.RestaurantId}")
            .SendAsync("NewOrderReceived", new
            {
                OrderId = notification.OrderId,
                OrderCode = notification.OrderCode,
                TableCode = notification.TableCode,
                Items = notification.Items,
                CustomerName = notification.CustomerName,
                Notes = notification.Notes,
                CreatedAt = notification.CreatedAt
            });
    }
}
```

## ⚙️ Configuración de la Aplicación

### Program.cs
```csharp
var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Restaurant Management API",
        Description = "API para gestión integral de restaurantes"
    });
    
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
});

// CORS configuration
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

// SignalR
builder.Services.AddSignalR();

// Clean Architecture layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure pipeline
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Restaurant Management API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<KitchenHub>("/hubs/kitchen");

app.Run();
```

### appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=RestaurantMgDB;Trusted_Connection=true;TrustServerCertificate=true;"
  },
  "JwtSettings": {
    "SecretKey": "your-super-secret-key-that-is-long-enough",
    "ExpiryInMinutes": 60
  },
  "Firebase": {
    "Type": "service_account",
    "ProjectId": "restaurant-management-xxxx",
    "StorageBucket": "restaurant-management-xxxx.appspot.com"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  }
}
```

## 🔒 Seguridad

### Autenticación JWT
- Tokens seguros con HS256
- Claims personalizados (UserId, RestaurantId, Role)
- Expiración configurable
- Refresh token para renovación

### Autorización por Roles
```csharp
[Authorize(Roles = "Admin")]           // Solo administradores
[Authorize(Roles = "Admin,Kitchen")]   // Admin o cocina
[AllowAnonymous]                       // Endpoint público
```

### 🔐 Diagrama de Flujo: Autenticación y Autorización

**¿Cómo funciona la seguridad paso a paso?**

```
1. LOGIN INICIAL:
┌─────────────────┐
│  👤 Usuario     │  POST /api/auth/login
│  (admin/cocina) │ ─────────────────────┐
│  email+password │                      ▼
└─────────────────┘            ┌─────────────────┐
                               │ AuthController  │
                               │ • Valida creds  │
                               │ • Consulta BD   │
                               └─────────┬───────┘
                                         │ Si válido
                                         ▼
                               ┌─────────────────┐
                               │   JwtService    │
                               │ Genera token:   │
                               │ {               │
                               │  "userId": 123, │
                               │  "restaurantId":│
                               │    "rest-456",  │
                               │  "role": "Admin"│
                               │ }               │
                               └─────────┬───────┘
                                         │ HTTP 200 + token
                                         ▼
┌─────────────────┐              ┌─────────────────┐
│  👤 Usuario     │ ◄────────────│   Response      │
│  guarda token   │              │ {               │
│  en su app      │              │  "token": "..."  │
│                 │              │ }               │
└─────────────────┘              └─────────────────┘

2. PETICIONES AUTENTICADAS:
┌─────────────────┐
│  📱 App         │  GET /api/kitchen/orders
│  incluye:       │  Authorization: Bearer {token}
│  Bearer token   │ ─────────────────────────────────┐
└─────────────────┘                                  ▼
                                         ┌─────────────────┐
                                         │ Middleware JWT  │
                                         │ • Valida token  │
                                         │ • Extrae claims │
                                         │ • Verifica role │
                                         └─────────┬───────┘
                                                   │ Si autorizado
                                                   ▼
                                         ┌─────────────────┐
                                         │KitchenController│
                                         │[Authorize(Roles:│
                                         │"Admin,Kitchen")]│
                                         │                 │
                                         │ ✅ ACCESO       │
                                         │    PERMITIDO    │
                                         └─────────────────┘

3. CONTROL DE ACCESO POR RESTAURANTE:
┌─────────────────┐
│CurrentUserService│ ← Token contiene restaurantId
│ • RestaurantId  │
│ • UserId        │
│ • Role          │
└─────────┬───────┘
          │ Inyectado en handlers
          ▼
┌─────────────────┐
│  GetKitchenOrders│
│  Handler        │
│                 │
│ var restaurantId│
│   = user.       │
│   RestaurantId; │
│                 │
│ query.Where(o =>│
│  o.RestaurantId │
│  == restaurantId│
│ );              │
└─────────────────┘

🔒 RESULTADO: Usuario solo ve datos de SU restaurante
```

### 🌊 Flujo de Datos Completo: Cliente → Base de Datos

```
EJEMPLO: Cliente consulta estado de su pedido

📱 Cliente                🌐 API                🎯 Application           📊 Database
    │                        │                      │                       │
    │ GET /orders/ABC123/     │                      │                       │
    │ status                  │                      │                       │
    ├────────────────────────►│                      │                       │
    │                        │ 1. OrdersController   │                       │
    │                        │    recibe petición    │                       │
    │                        │                      │                       │
    │                        │ 2. Crea Query        │                       │
    │                        ├─────────────────────►│                       │
    │                        │   GetOrderByCode     │                       │
    │                        │                      │                       │
    │                        │                      │ 3. Handler ejecuta    │
    │                        │                      │    validaciones       │
    │                        │                      │                       │
    │                        │                      │ 4. Consulta BD       │
    │                        │                      ├──────────────────────►│
    │                        │                      │   SELECT * FROM       │
    │                        │                      │   Orders WHERE        │
    │                        │                      │   Code = 'ABC123'     │
    │                        │                      │                       │
    │                        │                      │ 5. Retorna Order      │
    │                        │                      │◄──────────────────────┤
    │                        │                      │   Order entity        │
    │                        │                      │                       │
    │                        │ 6. Convierte a DTO   │                       │
    │                        │◄─────────────────────┤                       │
    │                        │   OrderStatusDto     │                       │
    │                        │                      │                       │
    │ 7. HTTP 200 + JSON     │                      │                       │
    │◄───────────────────────┤                      │                       │
    │ {                      │                      │                       │
    │   "status": "Ready",   │                      │                       │
    │   "estimatedTime": 5   │                      │                       │
    │ }                      │                      │                       │

⚡ TIEMPO TOTAL: ~50-100ms (consulta simple)
```

### Validación de Entrada
- Data Annotations en DTOs
- Model State validation automática
- Sanitización de datos de entrada
- Límites de tamaño de archivos

### CORS Policy
- Configuración permisiva para desarrollo
- Restrictiva para producción
- Headers específicos permitidos

## 📋 Documentación API

### Swagger/OpenAPI
- Documentación automática de endpoints
- Esquemas de request/response
- Ejemplos de uso
- Autenticación JWT integrada

### Response Codes Estándar
- `200 OK`: Operación exitosa
- `201 Created`: Recurso creado exitosamente  
- `400 Bad Request`: Datos de entrada inválidos
- `401 Unauthorized`: Token inválido o faltante
- `403 Forbidden`: Sin permisos suficientes
- `404 Not Found`: Recurso no encontrado
- `500 Internal Server Error`: Error del servidor

### Headers Estándar
- `Content-Type: application/json`
- `Authorization: Bearer {token}`
- `Accept: application/json`

## 🧪 Pruebas y Monitoreo del Proyecto

### Estrategia de Testing Implementada
En este proyecto académico se realizaron **pruebas manuales exhaustivas** de todos los endpoints y funcionalidades de la API. Se priorizó validar el correcto funcionamiento de la arquitectura y la lógica de negocio.

**Pruebas manuales realizadas:**
- **Testing de Controllers**: Verificación manual de cada endpoint usando herramientas como Postman y Swagger UI
- **Testing de Autenticación**: Validación de tokens JWT, autorización por roles, y manejo de sesiones
- **Testing de Integración**: Verificación del flujo completo desde la API hasta la base de datos
- **Testing de SignalR**: Pruebas de comunicación en tiempo real entre clientes y hubs
- **Testing de Validación**: Verificación de DTOs, model validation, y manejo de errores

**Casos de prueba validados:**
- Creación de restaurantes y usuarios con validaciones correctas
- Flujo completo de pedidos desde cliente hasta cocina
- Autenticación y autorización en endpoints protegidos
- Manejo de errores HTTP apropiados (400, 401, 403, 404, 500)
- Funcionalidad de SignalR en tiempo real
- Integración con servicios externos (Firebase Storage)

**Herramientas utilizadas para testing:**
- **Swagger UI**: Para documentación interactiva y pruebas de endpoints
- **Postman**: Para testing manual de APIs y colecciones de pruebas
- **Browser Dev Tools**: Para testing de SignalR y WebSocket connections
- **SQL Server Management Studio**: Para verificar datos en base de datos

### Despliegue en Producción

**Plataforma de hosting: MonsterASP.NET**

**Proceso de despliegue ejecutado:**
1. **Preparación del build**: `dotnet publish -c Release --output ./publish`
2. **Configuración de producción**: Actualización de `appsettings.json` con cadena de conexión de MonsterASP.NET
3. **Creación de base de datos**: Setup de SQL Server en el panel de MonsterASP.NET
4. **Aplicación de migraciones**: Ejecución de `dotnet ef database update` con la cadena de producción
5. **Upload de archivos**: Subida de archivos del directorio `publish` al hosting
6. **Configuración de IIS**: Setup del archivo `web.config` para ASP.NET Core

**Configuración específica aplicada:**

```json
{
  "ConnectionStrings": {
    "DeployConnection": "Server=MonsterASP-SQL-Server;Database=RestaurantMgDB;User Id=username;Password=password;TrustServerCertificate=true;"
  },
  "JwtSettings": {
    "SecretKey": "production-secret-key",
    "ExpiryInMinutes": 60
  }
}
```

**Comando de migración utilizado:**
```bash
dotnet ef database update --connection "Server=MonsterASP-SQL-Server;Database=RestaurantMgDB;User Id=username;Password=password;TrustServerCertificate=true;" --project Infrastructure --startup-project API
```

### Monitoreo Básico Implementado

**Logging configurado:**
- ASP.NET Core logging para requests y responses
- Error logging para excepciones no controladas
- Logging básico en servicios críticos (Firebase, autenticación)

**Swagger habilitado en producción:**
- Documentación de API accesible en producción para testing
- Interfaz interactiva para probar endpoints en vivo

**En futuras iteraciones se implementaría:**
- **Testing automatizado**: Unit tests para controllers, integration tests end-to-end
- **Logging avanzado**: Structured logging con Serilog, correlation IDs
- **Monitoreo profesional**: Application Insights, health checks, performance metrics
- **CI/CD Pipeline**: Despliegue automatizado con GitHub Actions

## 📊 Monitoreo y Logging

### Application Insights
```csharp
builder.Services.AddApplicationInsightsTelemetry();
```

### Structured Logging
```csharp
_logger.LogInformation("Order {OrderId} created for table {TableId}", order.Id, order.TableId);
_logger.LogWarning("Failed login attempt for email {Email}", request.Email);
_logger.LogError(ex, "Error processing order {OrderId}", orderId);
```

### Health Checks
```csharp
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>()
    .AddCheck<FirebaseStorageHealthCheck>("firebase");

app.MapHealthChecks("/health");
```

## 🚀 Deployment

### Web.config (IIS)
```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <system.webServer>
    <handlers>
      <add name="aspNetCore" path="*" verb="*" modules="AspNetCoreModuleV2" resourceType="Unspecified" />
    </handlers>
    <aspNetCore processPath="dotnet" 
                arguments=".\API.dll" 
                stdoutLogEnabled="true" 
                stdoutLogFile=".\logs\stdout" 
                hostingModel="inprocess" />
  </system.webServer>
</configuration>
```

### Environment Variables
```bash
# Production
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://0.0.0.0:80
ConnectionStrings__DeployConnection="Server=..."
JwtSettings__SecretKey="production-secret-key"
```

---

## 🔗 Enlaces Relacionados

- [Domain Layer](../Domain/README.md)
- [Application Layer](../Application/README.md)  
- [Infrastructure Layer](../Infrastructure/README.md)
- [Documentación Principal](../README.md)
