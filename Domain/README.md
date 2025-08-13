# 🏛️ Domain Layer

> **Capa de Dominio - El Corazón del Negocio**

## 📋 Descripción

La capa de **Domain** constituye el núcleo del sistema, conteniendo todas las entidades de negocio, reglas de dominio y lógica fundamental. Esta capa es completamente independiente de tecnologías externas, frameworks o bases de datos, siguiendo los principios de Domain-Driven Design (DDD).

## 🎯 Principios de Diseño

### Domain-Driven Design (DDD)
El proyecto implementa principios fundamentales de DDD:

- **Ubiquitous Language**: Uso consistente del lenguaje de dominio del negocio restaurantero
- **Bounded Context**: Enfoque específico en el contexto de gestión de restaurantes
- **Rich Domain Model**: Entidades con comportamientos y validaciones de negocio integradas
- **Business Rules**: Reglas de negocio encapsuladas dentro de las entidades correspondientes

### Principios SOLID
- **Single Responsibility**: Cada entidad tiene una responsabilidad específica y bien definida
- **Open/Closed**: Diseño extensible que permite nuevas funcionalidades sin modificar código existente
- **Dependency Inversion**: Independencia completa de frameworks y tecnologías externas

## 📁 Estructura del Proyecto

```
Domain/
├── Models/           # Entidades del dominio
│   ├── Restaurant.cs
│   ├── Table.cs
│   ├── MenuItem.cs
│   ├── Category.cs
│   ├── Order.cs
│   ├── OrderItem.cs
│   └── User.cs
└── Enums/           # Enumeraciones del dominio
    ├── OrderStatus.cs
    ├── TableStatus.cs
    └── UserRole.cs
```

## 🏗️ Entidades del Dominio

### 🏪 Restaurant (Restaurante)
**Responsabilidad**: Representa un restaurante en el sistema

**Propiedades:**
- `Id`: Identificador único
- `Name`: Nombre del restaurante
- `BrandingColor`: Color principal de la marca
- `LogoUrl`: URL del logo en Firebase Storage
- `ClientUrl`: URL del frontend del cliente
- `CreatedAt`: Fecha de creación

**Relaciones:**
- Uno a muchos con `Table` (Mesas)
- Uno a muchos con `Category` (Categorías)
- Uno a muchos con `User` (Usuarios)
- Uno a muchos con `Order` (Órdenes)

**Reglas de Negocio:**
- Un restaurante debe tener al menos un usuario administrador
- El nombre del restaurante debe ser único en el sistema
- La URL del cliente debe ser válida y accesible

### 🪑 Table (Mesa)
**Responsabilidad**: Representa una mesa física del restaurante

**Propiedades:**
- `Id`: Identificador único
- `Code`: Código alfanumérico único (ej: "A01", "B05")
- `Status`: Estado actual de la mesa
- `RestaurantId`: Referencia al restaurante
- `CreatedAt`: Fecha de creación

**Relaciones:**
- Muchas a uno con `Restaurant`
- Uno a muchos con `Order`

**Reglas de Negocio:**
- El código debe ser único por restaurante
- Una mesa puede tener múltiples órdenes pero solo una activa
- El estado debe transicionar correctamente (Available → Occupied → Available)

### 🍽️ MenuItem (Elemento del Menú)
**Responsabilidad**: Representa un producto/plato del menú

**Propiedades:**
- `Id`: Identificador único
- `Name`: Nombre del plato
- `Description`: Descripción detallada
- `Price`: Precio en la moneda local
- `ImageUrl`: URL de la imagen en Firebase Storage
- `IsAvailable`: Disponibilidad actual
- `CategoryId`: Referencia a la categoría
- `RestaurantId`: Referencia al restaurante

**Relaciones:**
- Muchos a uno con `Category`
- Muchos a uno con `Restaurant`
- Uno a muchos con `OrderItem`

**Reglas de Negocio:**
- El precio debe ser mayor que cero
- El nombre debe ser único por categoría
- Un item no disponible no puede ser incluido en nuevas órdenes

### 📂 Category (Categoría)
**Responsabilidad**: Agrupa elementos del menú por tipo

**Propiedades:**
- `Id`: Identificador único
- `Name`: Nombre de la categoría
- `DisplayOrder`: Orden de visualización
- `RestaurantId`: Referencia al restaurante

**Relaciones:**
- Muchas a uno con `Restaurant`
- Uno a muchos con `MenuItem`

**Reglas de Negocio:**
- El nombre debe ser único por restaurante
- El orden de visualización debe ser único por restaurante
- No se puede eliminar una categoría con elementos asociados

### 📱 Order (Orden)
**Responsabilidad**: Representa un pedido de un cliente

**Propiedades:**
- `Id`: Identificador único
- `Code`: Código alfanumérico para seguimiento
- `Status`: Estado actual de la orden
- `Total`: Monto total calculado
- `CustomerName`: Nombre del cliente (opcional)
- `Notes`: Notas especiales del pedido
- `TableId`: Referencia a la mesa
- `RestaurantId`: Referencia al restaurante
- `CreatedAt`: Fecha de creación
- `CompletedAt`: Fecha de finalización

**Relaciones:**
- Muchas a uno con `Table`
- Muchas a uno con `Restaurant`
- Uno a muchos con `OrderItem`

**Reglas de Negocio:**
- Una orden debe tener al menos un item
- El total debe ser calculado automáticamente
- El código debe ser único y fácil de recordar (4-6 caracteres)
- Los estados deben seguir el flujo: Pending → InPreparation → Ready → Completed

### 🛒 OrderItem (Item de Orden)
**Responsabilidad**: Representa un elemento específico dentro de una orden

**Propiedades:**
- `Id`: Identificador único
- `Quantity`: Cantidad solicitada
- `UnitPrice`: Precio unitario al momento de la orden
- `Notes`: Notas específicas del item
- `OrderId`: Referencia a la orden
- `MenuItemId`: Referencia al elemento del menú

**Relaciones:**
- Muchos a uno con `Order`
- Muchos a uno con `MenuItem`

**Reglas de Negocio:**
- La cantidad debe ser mayor que cero
- El precio unitario se congela al momento de crear la orden
- Las notas son opcionales pero útiles para especificaciones especiales

### 👤 User (Usuario)
**Responsabilidad**: Representa usuarios del sistema de gestión

**Propiedades:**
- `Id`: Identificador único
- `Email`: Email único para autenticación
- `PasswordHash`: Hash de la contraseña
- `FirstName`: Nombre
- `LastName`: Apellido
- `Role`: Rol en el sistema
- `RestaurantId`: Referencia al restaurante
- `CreatedAt`: Fecha de creación

**Relaciones:**
- Muchos a uno con `Restaurant`

**Reglas de Negocio:**
- El email debe ser único en todo el sistema
- La contraseña debe cumplir requisitos mínimos de seguridad
- Un usuario Admin puede gestionar todo el restaurante
- Un usuario Kitchen solo puede ver órdenes de cocina

## 🔢 Enumeraciones del Dominio

### OrderStatus (Estado de Orden)
```csharp
public enum OrderStatus
{
    Pending = 0,        // Pendiente de cocina
    InPreparation = 1,  // En preparación
    Ready = 2,          // Listo para entrega
    Completed = 3       // Completado/Entregado
}
```

**Transiciones válidas:**
- `Pending` → `InPreparation`
- `InPreparation` → `Ready`
- `Ready` → `Completed`

### TableStatus (Estado de Mesa)
```csharp
public enum TableStatus
{
    Available = 0,  // Disponible
    Occupied = 1    // Ocupada
}
```

**Uso:**
- `Available`: Mesa libre para nuevos clientes
- `Occupied`: Mesa con orden activa

### UserRole (Rol de Usuario)
```csharp
public enum UserRole
{
    Admin = 0,    // Administrador del restaurante
    Kitchen = 1   // Personal de cocina
}
```

**Permisos:**
- `Admin`: Acceso completo al sistema
- `Kitchen`: Solo módulo de cocina y órdenes

## 🔧 Reglas de Negocio Transversales

### Validaciones de Dominio

#### Restaurant
- ✅ Nombre requerido y único
- ✅ Logo opcional pero URL válida si se proporciona
- ✅ Al menos un usuario administrador

#### Order Flow
- ✅ Solo items disponibles pueden ser ordenados
- ✅ Precio congelado al momento de la orden
- ✅ Cálculo automático del total
- ✅ Flujo de estados controlado

#### Table Management
- ✅ Códigos únicos por restaurante
- ✅ Solo una orden activa por mesa
- ✅ Estados controlados

#### User Security
- ✅ Emails únicos globalmente
- ✅ Contraseñas hasheadas con BCrypt
- ✅ Roles claramente definidos

### Invariantes del Dominio

1. **Consistencia de Precios**: El precio en `OrderItem` no puede cambiar después de creada la orden
2. **Disponibilidad**: Solo items `IsAvailable = true` pueden ser ordenados
3. **Estados Válidos**: Las transiciones de estado deben seguir las reglas definidas
4. **Integridad Referencial**: Todas las relaciones deben ser válidas

## 🏷️ Value Objects (Conceptos)

Aunque no implementados como clases separadas, estos conceptos son importantes:

### Money (Dinero)
- Representado como `decimal` para precisión
- Siempre en la moneda local del restaurante
- Validación: debe ser >= 0

### Code (Código)
- Códigos alfanuméricos únicos
- Fáciles de recordar y comunicar
- Validación: longitud y caracteres permitidos

### Email
- Formato válido requerido
- Único en todo el sistema
- Normalizado a lowercase

## 🧪 Testing y Logging (No Implementados en Este Proyecto Académico)

### ¿Por qué no incluimos Testing del Dominio?
En este proyecto académico nos enfocamos en demostrar las entidades y reglas de negocio. En un entorno de producción real incluiríamos:

**Testing que se implementaría:**
- **Entity Tests**: Pruebas de cada entidad (Restaurant, Order, etc.) por separado
- **Business Rule Tests**: Verificación de que las reglas de negocio se cumplan (ej: "un pedido debe tener al menos un item")
- **State Transition Tests**: Pruebas de los cambios de estado (ej: Pending → InPreparation → Ready → Completed)

**Ejemplo de lo que se probaría:**
- ¿Se calcula correctamente el total de un pedido?
- ¿Se impide crear un pedido con items no disponibles?
- ¿Las transiciones de estado siguen las reglas definidas?

### ¿Por qué no incluimos Logging del Dominio?
El logging del dominio registraría cuando se aplican las reglas de negocio. En producción incluiríamos:

**Logging que se implementaría:**
- **Business Rule Logging**: Registrar cuándo y por qué se aplican ciertas reglas
- **State Change Logging**: Registrar todos los cambios de estado con razones
- **Validation Logging**: Registrar cuándo se rechaza algo por no cumplir las reglas
- **Domain Event Logging**: Registrar eventos importantes del dominio (nuevo pedido, cambio de estado)

### ¿Por qué no incluimos Telemetría del Dominio?
La telemetría del dominio nos ayudaría a entender cómo se comporta el negocio. En producción incluiríamos:

**Telemetría que se implementaría:**
- **Business Metrics**: Estadísticas de negocio (pedidos por hora, productos más vendidos)
- **Rule Application Metrics**: Frecuencia de aplicación de cada regla de negocio
- **State Transition Metrics**: Estadísticas de los flujos de estado
- **Domain Performance Metrics**: Tiempos de ejecución de las operaciones de dominio

## 📊 Diagramas de Dominio

### Diagrama de Entidades
```
Restaurant
    │
    ├─── Users (1:N)
    ├─── Tables (1:N)
    ├─── Categories (1:N)
    ├─── MenuItems (1:N)
    └─── Orders (1:N)

Table
    └─── Orders (1:N)

Category
    └─── MenuItems (1:N)

MenuItem
    └─── OrderItems (1:N)

Order
    └─── OrderItems (1:N)
```

### Flujo de Estados de Order
```
[Pending] → [InPreparation] → [Ready] → [Completed]
```

## 🔗 Principios de Clean Architecture

### Independencia Tecnológica
- No referencias a frameworks externos
- No dependencias de infraestructura
- Lógica pura de negocio

### Testabilidad
- Entidades completamente testables
- Sin dependencias externas
- Reglas de negocio verificables

### Evolución
- Fácil de extender con nuevos conceptos
- Resistente a cambios tecnológicos
- Base sólida para el crecimiento

---

## 🔗 Enlaces Relacionados

- [Application Layer](../Application/README.md)
- [Infrastructure Layer](../Infrastructure/README.md)
- [API Layer](../API/README.md)
- [Documentación Principal](../README.md)
