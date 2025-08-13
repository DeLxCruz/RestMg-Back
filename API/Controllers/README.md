# 🎮 Controllers - Endpoints de la API

> **Controladores REST - Puntos de Entrada del Sistema**

## 📋 Descripción

Los controladores son los "porteros" de la API. Cada controlador maneja un conjunto específico de funcionalidades del sistema de restaurante. Reciben las peticiones HTTP, las validan, y las envían a la capa de aplicación para procesarlas.

## 🔐 AuthController

**¿Qué hace?** Maneja todo lo relacionado con autenticación de usuarios.

### Endpoints

#### `POST /api/auth/login`
- **Propósito**: Autenticar un usuario (admin, cocina, mesero)
- **Entrada**: Email y contraseña
- **Salida**: Token JWT para hacer peticiones autenticadas
- **Uso típico**: Pantalla de login en cualquier aplicación del sistema

**Ejemplo de uso:**
```json
POST /api/auth/login
{
    "email": "admin@restaurante.com",
    "password": "mipassword123"
}
```

**¿Qué pasa internamente?**
1. Verifica que el email y password sean correctos
2. Genera un token JWT con información del usuario y restaurante
3. Devuelve el token para usar en futuras peticiones

---

## 🏪 RestaurantsController

**¿Qué hace?** Gestiona la información básica de los restaurantes.

### Endpoints

#### `POST /api/restaurants/onboard`
- **Propósito**: Crear un restaurante nuevo con su administrador
- **Entrada**: Datos del restaurante (nombre, color, admin)
- **Uso típico**: Proceso de registro de un restaurante nuevo al sistema

#### `GET /api/restaurants/{id}`
- **Propósito**: Obtener información de un restaurante específico
- **Salida**: Nombre, colores, logo, etc.
- **Uso típico**: Mostrar información del restaurante en el dashboard

#### `PUT /api/restaurants/{id}`
- **Propósito**: Actualizar información del restaurante
- **Uso típico**: Cambiar nombre, colores, o información general

---

## 🪑 TablesController

**¿Qué hace?** Administra las mesas del restaurante.

### Endpoints

#### `GET /api/tables`
- **Propósito**: Listar todas las mesas del restaurante
- **Salida**: Lista con código, estado (libre/ocupada) de cada mesa
- **Uso típico**: Panel de administración para ver estado de mesas

#### `POST /api/tables`
- **Propósito**: Crear una mesa nueva
- **Entrada**: Código de la mesa
- **Uso típico**: Configuración inicial del restaurante

#### `GET /api/tables/{id}/qr`
- **Propósito**: Generar código QR de una mesa específica
- **Salida**: Imagen PNG del código QR
- **Uso típico**: Imprimir códigos QR para poner en las mesas físicas

**¿Cómo funciona el QR?**
El código QR contiene una URL que lleva a los clientes directamente al menú de ese restaurante y mesa específica.

---

## 🍽️ MenuController

**¿Qué hace?** Permite a los CLIENTES ver el menú del restaurante. (Es público, no requiere autenticación)

### Endpoints

#### `GET /api/menu/restaurant/{restaurantId}`
- **Propósito**: Mostrar el menú completo a los clientes
- **Salida**: Categorías, platos, precios, imágenes
- **Uso típico**: Cuando un cliente escanea el QR de la mesa

#### `GET /api/menu/restaurant/{restaurantId}/categories`
- **Propósito**: Obtener solo las categorías del menú
- **Uso típico**: Para mostrar filtros por categoría en la app del cliente

**¿Quién usa esto?**
Los CLIENTES del restaurante cuando navegan el menú desde sus teléfonos.

---

## ⚙️ MenuManagementController

**¿Qué hace?** Permite a los ADMINISTRADORES gestionar el menú. (Requiere autenticación)

### Endpoints

#### `GET /api/menu-management/categories`
- **Propósito**: Listar categorías para administrar
- **Uso típico**: Panel de administración del menú

#### `POST /api/menu-management/categories`
- **Propósito**: Crear categoría nueva (ej: "Entradas", "Platos principales")
- **Uso típico**: Configuración inicial o expansión del menú

#### `POST /api/menu-management/menu-items`
- **Propósito**: Agregar un plato nuevo al menú
- **Entrada**: Nombre, descripción, precio, imagen, categoría
- **Uso típico**: Cuando el restaurante quiere agregar un plato nuevo

#### `PUT /api/menu-management/menu-items/{id}/availability`
- **Propósito**: Marcar un plato como disponible o agotado
- **Uso típico**: Cuando se agota un ingrediente y no pueden preparar el plato

**¿Quién usa esto?**
Los ADMINISTRADORES del restaurante para gestionar su menú.

---

## 📱 OrdersController

**¿Qué hace?** Gestiona los pedidos de los clientes.

### Endpoints

#### `POST /api/orders` (PÚBLICO)
- **Propósito**: Crear un pedido nuevo
- **Entrada**: Mesa, platos seleccionados, nombre del cliente (opcional)
- **Salida**: Código del pedido para seguimiento
- **Uso típico**: Cuando un cliente confirma su pedido desde el menú

#### `GET /api/orders/{code}/status` (PÚBLICO)
- **Propósito**: Ver el estado de un pedido
- **Entrada**: Código del pedido
- **Salida**: Estado actual (Pendiente, En preparación, Listo, Entregado)
- **Uso típico**: Cliente verifica cuándo estará listo su pedido

#### `GET /api/orders/restaurant/{restaurantId}` (AUTENTICADO)
- **Propósito**: Ver todos los pedidos del restaurante
- **Uso típico**: Panel de administración para ver historial de pedidos

**Estados de un pedido:**
1. **AwaitingPayment**: Esperando que el cliente pague
2. **Pending**: Pagado, esperando que cocina lo vea
3. **InPreparation**: Cocina está preparando
4. **Ready**: Listo para entregar
5. **Delivered**: Entregado al cliente

---

## 👨‍🍳 KitchenController

**¿Qué hace?** Herramientas específicas para la cocina del restaurante.

### Endpoints

#### `GET /api/kitchen/orders`
- **Propósito**: Ver pedidos que debe preparar la cocina
- **Salida**: Lista de pedidos con estado Pending o InPreparation
- **Uso típico**: Pantalla de cocina para ver qué cocinar

#### `PUT /api/kitchen/orders/{id}/start`
- **Propósito**: Marcar que cocina empezó a preparar un pedido
- **Efecto**: Cambia estado de Pending → InPreparation
- **Uso típico**: Cocina confirma que empezó a cocinar

#### `PUT /api/kitchen/orders/{id}/ready`
- **Propósito**: Marcar que un pedido está listo
- **Efecto**: Cambia estado de InPreparation → Ready
- **Uso típico**: Cocina confirma que terminó de cocinar

#### `PUT /api/kitchen/orders/{id}/confirm-payment`
- **Propósito**: Confirmar que el cliente pagó
- **Efecto**: Cambia estado de AwaitingPayment → Pending
- **Uso típico**: Mesero confirma que cliente pagó y pedido pasa a cocina

#### `GET /api/kitchen/history/today`
- **Propósito**: Ver historial de pedidos completados hoy
- **Uso típico**: Revisar productividad del día

**¿Quién usa esto?**
Personal de COCINA y ADMINISTRADORES con acceso a cocina.

---

## 🎨 BrandingController

**¿Qué hace?** Maneja imágenes y elementos visuales del restaurante.

### Endpoints

#### `POST /api/branding/upload-logo`
- **Propósito**: Subir el logo del restaurante
- **Entrada**: Archivo de imagen
- **Uso típico**: Configuración del restaurante

#### `POST /api/branding/menu-items/{id}/upload-image`
- **Propósito**: Subir foto de un plato del menú
- **Entrada**: Archivo de imagen
- **Uso típico**: Hacer el menú más atractivo con fotos

**¿Dónde se guardan las imágenes?**
En Firebase Storage (servicio de Google Cloud).

---

## 👥 UsersController

**¿Qué hace?** Administrar empleados del restaurante.

### Endpoints

#### `GET /api/users`
- **Propósito**: Listar empleados del restaurante
- **Uso típico**: Panel de administración de personal

#### `POST /api/users`
- **Propósito**: Crear empleado nuevo
- **Entrada**: Email, password, rol (Admin, Kitchen)
- **Uso típico**: Contratar personal nuevo

#### `PUT /api/users/{id}`
- **Propósito**: Actualizar información de un empleado
- **Uso típico**: Cambiar rol o información personal

#### `DELETE /api/users/{id}`
- **Propósito**: Eliminar empleado
- **Uso típico**: Cuando alguien ya no trabaja en el restaurante

**Roles disponibles:**
- **Admin**: Puede hacer todo
- **Kitchen**: Solo puede ver y gestionar pedidos de cocina

---

## 📊 ReportsController

**¿Qué hace?** Genera reportes y estadísticas del restaurante.

### Endpoints

#### `GET /api/reports/sales/today`
- **Propósito**: Ver ventas del día
- **Salida**: Total vendido, número de pedidos
- **Uso típico**: Revisar cómo va el día comercialmente

#### `GET /api/reports/sales/monthly`
- **Propósito**: Ver ventas del mes
- **Uso típico**: Análisis mensual de performance

#### `GET /api/reports/popular-items`
- **Propósito**: Ver qué platos se venden más
- **Uso típico**: Decidir qué mantener en el menú

**¿Para qué sirven los reportes?**
Para que los dueños del restaurante tomen decisiones de negocio basadas en datos reales.

---

## 🔒 Seguridad de los Endpoints

### Endpoints Públicos (No requieren autenticación)
- **MenuController**: Clientes pueden ver el menú sin registrarse
- **OrdersController**: Crear y consultar pedidos (algunos endpoints)

### Endpoints Autenticados (Requieren token JWT)
- **Todos los demás controladores**: Solo empleados autenticados

### Roles Específicos
- **Admin**: Puede usar todos los endpoints
- **Kitchen**: Solo KitchenController y algunos de OrdersController

### ¿Cómo funciona la autenticación?
1. Usuario hace login → Recibe token JWT
2. Incluye token en header: `Authorization: Bearer {token}`
3. API verifica token antes de procesar petición

---

## 🚦 Códigos de Respuesta HTTP

### Respuestas Exitosas
- **200 OK**: Todo funcionó correctamente
- **201 Created**: Se creó algo nuevo (restaurante, usuario, pedido)

### Errores del Cliente
- **400 Bad Request**: Datos inválidos (email mal formateado, campos faltantes)
- **401 Unauthorized**: Token inválido o faltante
- **403 Forbidden**: No tienes permisos (ej: cocina intentando crear usuarios)
- **404 Not Found**: No existe (restaurante, pedido, usuario)

### Errores del Servidor
- **500 Internal Server Error**: Algo falló en el sistema

---

## 📱 ¿Quién usa cada controlador?

### Para Clientes del Restaurante
- **MenuController**: Ver menú y hacer pedidos
- **OrdersController**: Seguir estado de sus pedidos

### Para Empleados del Restaurante
- **KitchenController**: Personal de cocina
- **AuthController**: Todos para hacer login
- **MenuManagementController**: Administradores
- **UsersController**: Administradores
- **ReportsController**: Administradores
- **BrandingController**: Administradores
- **TablesController**: Administradores
- **RestaurantsController**: Administradores
