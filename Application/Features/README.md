# 🚀 Features - Casos de Uso del Sistema

> **Funcionalidades del Negocio - Lo Que Puede Hacer El Sistema**

## 📋 Descripción

Las Features contienen toda la lógica de negocio del sistema. Cada feature representa algo específico que puede hacer un usuario del sistema. Están organizadas usando el patrón CQRS: Commands (para hacer cambios) y Queries (para obtener información).

---

## 🔐 Auth - Autenticación de Usuarios

**¿Qué hace?** Permite a los empleados del restaurante iniciar sesión en el sistema.

### Commands (Acciones)

#### Login
- **Qué hace**: Verifica credenciales y genera token de acceso
- **Quién lo usa**: Cualquier empleado (admin, cocina) que necesite acceder al sistema
- **Resultado**: Token JWT para hacer peticiones autenticadas
- **Validaciones**: Email válido, contraseña correcta, usuario activo

**¿Por qué es importante?**
Sin autenticación, cualquiera podría acceder al sistema del restaurante y ver pedidos, cambiar precios, etc.

---

## 🎨 Branding - Imagen Visual del Restaurante

**¿Qué hace?** Gestiona los elementos visuales que identifican al restaurante (logos, imágenes de platos).

### Commands (Acciones)

#### UploadLogo
- **Qué hace**: Sube el logo del restaurante a la nube (Firebase)
- **Quién lo usa**: Administradores configurando su restaurante
- **Resultado**: URL del logo para mostrar en el menú digital
- **Validaciones**: Solo imágenes válidas, tamaño máximo permitido

#### UploadMenuItemImage
- **Qué hace**: Sube foto de un plato específico del menú
- **Quién lo usa**: Administradores mejorando la presentación del menú
- **Resultado**: URL de la imagen para mostrar junto al plato
- **Validaciones**: Imagen válida, plato debe existir

**¿Por qué es importante?**
Los clientes deciden qué pedir basándose en cómo se ve. Fotos atractivas aumentan las ventas.

---

## 📂 Categories - Organización del Menú

**¿Qué hace?** Organiza los platos del menú en categorías (Entradas, Platos Principales, Postres, etc.).

### Commands (Acciones)

#### CreateCategory
- **Qué hace**: Crea una nueva categoría para organizar platos
- **Quién lo usa**: Administradores organizando su menú
- **Ejemplo**: Crear categoría "Bebidas" para organizar jugos, sodas, etc.
- **Validaciones**: Nombre único dentro del restaurante

#### UpdateCategory
- **Qué hace**: Cambia el nombre o descripción de una categoría
- **Uso típico**: Cambiar "Bebidas" por "Bebidas y Jugos Naturales"

#### DeleteCategory
- **Qué hace**: Elimina una categoría que ya no se usa
- **Restricción**: Solo se puede eliminar si no tiene platos asociados

### Queries (Consultas)

#### GetCategories
- **Qué hace**: Lista todas las categorías del restaurante
- **Quién lo usa**: Para mostrar filtros en el menú o en la administración

**¿Por qué es importante?**
Un menú bien organizado es más fácil de navegar. Los clientes encuentran rápido lo que buscan.

---

## 👨‍🍳 Kitchen - Operaciones de Cocina

**¿Qué hace?** Gestiona todo el flujo de trabajo de la cocina del restaurante.

### Commands (Acciones)

#### StartOrder
- **Qué hace**: Marca que la cocina empezó a preparar un pedido
- **Quién lo usa**: Cocineros cuando empiezan a cocinar
- **Efecto**: Cambia estado de "Pendiente" → "En Preparación"
- **Notificación**: Informa a administradores que pedido está en proceso

#### MarkOrderReady
- **Qué hace**: Marca que un pedido está listo para entregar
- **Quién lo usa**: Cocineros cuando terminan de preparar
- **Efecto**: Cambia estado de "En Preparación" → "Listo"
- **Notificación**: Alerta a meseros que pueden entregar el pedido

#### ConfirmOrderPayment
- **Qué hace**: Confirma que el cliente pagó su pedido
- **Quién lo usa**: Meseros o administradores al recibir el pago
- **Efecto**: Envía el pedido a la cola de cocina para preparar
- **Notificación**: Aparece nuevo pedido en pantalla de cocina

### Queries (Consultas)

#### GetKitchenOrders
- **Qué hace**: Lista pedidos que la cocina debe preparar
- **Filtros**: Por estado (pendientes, en preparación), límite de resultados
- **Uso típico**: Pantalla de cocina mostrando qué cocinar

#### GetKitchenHistory
- **Qué hace**: Muestra historial de pedidos completados
- **Información**: Pedidos del día, tiempo promedio de preparación
- **Uso típico**: Revisar productividad de la cocina

**¿Por qué es importante?**
La cocina es el corazón del restaurante. Un flujo bien organizado significa clientes satisfechos y menos estrés para el personal.

---

## 🍽️ Menu - Visualización para Clientes

**¿Qué hace?** Permite a los clientes ver el menú del restaurante desde sus teléfonos.

### Queries (Consultas)

#### GetFullMenu
- **Qué hace**: Muestra el menú completo con todos los platos disponibles
- **Información**: Categorías, platos, precios, descripciones, fotos
- **Quién lo usa**: Clientes que escanearon el QR de la mesa
- **Filtros**: Solo platos disponibles (no agotados)

#### GetCategoriesByRestaurant
- **Qué hace**: Lista las categorías del menú para navegación
- **Uso típico**: Mostrar filtros para que el cliente pueda ver solo "Entradas" o "Postres"

**¿Por qué es importante?**
Es la primera impresión que tienen los clientes del restaurante. Un menú claro y atractivo aumenta las ventas.

---

## 🍕 MenuItems - Gestión de Platos

**¿Qué hace?** Administra los platos individuales del menú del restaurante.

### Commands (Acciones)

#### CreateMenuItem
- **Qué hace**: Agrega un plato nuevo al menú
- **Información**: Nombre, descripción, precio, categoría, imagen
- **Quién lo usa**: Administradores expandiendo su menú
- **Ejemplo**: Agregar "Hamburguesa BBQ" a la categoría "Platos Principales"

#### UpdateMenuItem
- **Qué hace**: Modifica información de un plato existente
- **Cambios típicos**: Precio, descripción, ingredientes
- **Uso**: Cuando cambian los precios o mejoran una receta

#### UpdateMenuItemAvailability
- **Qué hace**: Marca un plato como disponible o agotado
- **Quién lo usa**: Cocina cuando se agota un ingrediente
- **Efecto**: El plato desaparece temporalmente del menú de clientes
- **Notificación**: Informa a todas las pantallas del cambio

#### DeleteMenuItem
- **Qué hace**: Elimina un plato permanentemente del menú
- **Uso típico**: Platos que ya no se van a ofrecer más

### Queries (Consultas)

#### GetMenuItems
- **Qué hace**: Lista todos los platos del restaurante
- **Filtros**: Por categoría, por disponibilidad
- **Uso**: Administración del menú

#### GetMenuItemById
- **Qué hace**: Obtiene información detallada de un plato específico
- **Uso**: Para editar o ver detalles completos

**¿Por qué es importante?**
Los platos son lo que genera ingresos. Una gestión eficiente significa menos confusiones y más ventas.

---

## 📦 Orders - Gestión de Pedidos

**¿Qué hace?** Maneja todo el ciclo de vida de los pedidos de los clientes.

### Commands (Acciones)

#### CreateOrder
- **Qué hace**: Crea un pedido nuevo de un cliente
- **Información**: Mesa, platos seleccionados, cantidades, notas especiales
- **Validaciones**: Mesa existe, platos disponibles, cantidades válidas
- **Resultado**: Código único para que el cliente siga su pedido
- **Estado inicial**: "Esperando Pago"

**¿Cómo funciona un pedido completo?**
1. Cliente selecciona platos → CreateOrder (Estado: AwaitingPayment)
2. Cliente paga → ConfirmOrderPayment (Estado: Pending)
3. Cocina ve pedido → StartOrder (Estado: InPreparation)
4. Cocina termina → MarkOrderReady (Estado: Ready)
5. Mesero entrega → (Estado: Delivered)

### Queries (Consultas)

#### GetOrdersByRestaurant
- **Qué hace**: Lista todos los pedidos de un restaurante
- **Filtros**: Por fecha, por estado, por mesa
- **Uso**: Panel de administración, reportes

#### GetOrderByCode
- **Qué hace**: Busca un pedido específico por su código
- **Uso**: Cliente consultando estado de su pedido
- **Información**: Estado actual, tiempo estimado, items pedidos

#### GetOrderById
- **Qué hace**: Obtiene pedido por ID interno del sistema
- **Uso**: Operaciones internas del restaurante

**¿Por qué es importante?**
Los pedidos son la razón de ser del sistema. Un manejo eficiente significa clientes satisfechos y operaciones fluidas.

---

## 🏢 Restaurants - Configuración del Restaurante

**¿Qué hace?** Gestiona la información básica y configuración de cada restaurante en el sistema.

### Commands (Acciones)

#### CreateRestaurant (Onboard)
- **Qué hace**: Registra un restaurante nuevo en el sistema
- **Información**: Nombre, color de marca, datos del administrador inicial
- **Proceso**: Crea restaurante + crea usuario administrador + configura accesos
- **Uso típico**: Proceso de registro para restaurantes nuevos

#### UpdateRestaurant
- **Qué hace**: Actualiza información general del restaurante
- **Cambios típicos**: Nombre, colores de marca, información de contacto
- **Quién lo usa**: Administradores actualizando su información

### Queries (Consultas)

#### GetRestaurantById
- **Qué hace**: Obtiene información completa de un restaurante
- **Información**: Nombre, colores, logo, configuraciones
- **Uso**: Mostrar información en dashboards y menús

#### GetRestaurantByUserId
- **Qué hace**: Encuentra el restaurante al que pertenece un usuario
- **Uso**: Determinar permisos y filtrar información por restaurante

**¿Por qué es importante?**
Cada restaurante es independiente en el sistema. Esta configuración asegura que cada uno mantenga su identidad y datos separados.

---

## 📊 Reports - Análisis y Reportes

**¿Qué hace?** Genera estadísticas e información analítica para que los dueños tomen decisiones de negocio.

### Queries (Consultas)

#### GetSalesReport
- **Qué hace**: Genera reporte de ventas por período
- **Información**: Total vendido, número de pedidos, promedio por pedido
- **Períodos**: Diario, semanal, mensual
- **Uso típico**: Revisar performance financiero

#### GetPopularItems
- **Qué hace**: Lista los platos más vendidos
- **Información**: Plato, cantidad vendida, ingresos generados
- **Uso típico**: Decidir qué platos promocionar o cuáles quitar del menú

#### GetKitchenPerformance
- **Qué hace**: Analiza eficiencia de la cocina
- **Información**: Tiempo promedio de preparación, pedidos por hora
- **Uso típico**: Identificar cuellos de botella en la cocina

#### GetCustomerAnalytics
- **Qué hace**: Analiza patrones de comportamiento de clientes
- **Información**: Horarios pico, mesas más usadas, tickets promedio
- **Uso típico**: Optimizar operaciones y personal

**¿Por qué es importante?**
Los datos permiten tomar decisiones inteligentes. Un restaurante que analiza sus números puede mejorar continuamente y ser más rentable.

---

## 🪑 Tables - Gestión de Mesas

**¿Qué hace?** Administra las mesas físicas del restaurante y su estado en tiempo real.

### Commands (Acciones)

#### CreateTable
- **Qué hace**: Registra una mesa nueva en el sistema
- **Información**: Código único de la mesa (ej: "M01", "Mesa 5")
- **Resultado**: Genera QR code único para esa mesa
- **Uso típico**: Configuración inicial o al agregar mesas nuevas

#### UpdateTable
- **Qué hace**: Modifica información de una mesa
- **Cambios típicos**: Código, capacidad, ubicación
- **Uso**: Reorganización del restaurante

#### DeleteTable
- **Qué hace**: Elimina una mesa del sistema
- **Uso**: Cuando se remueve una mesa física del restaurante

### Queries (Consultas)

#### GetTablesByRestaurant
- **Qué hace**: Lista todas las mesas de un restaurante
- **Información**: Código, estado (libre/ocupada), pedidos actuales
- **Uso**: Panel de control para ver ocupación en tiempo real

#### GetTableById
- **Qué hace**: Obtiene información de una mesa específica
- **Uso**: Generar QR codes, verificar estado

#### GetTableQRCode
- **Qué hace**: Genera imagen QR para una mesa específica
- **Resultado**: Imagen PNG lista para imprimir
- **Uso**: Colocar códigos QR físicos en las mesas

**Estados de una mesa:**
- **Available**: Libre, lista para clientes
- **Occupied**: Ocupada, con pedido activo
- **Reserved**: Reservada (función futura)

**¿Por qué es importante?**
Las mesas son el punto de conexión entre el mundo físico y digital. Un buen control significa mejor experiencia para el cliente.

---

## 👥 Users - Gestión de Personal

**¿Qué hace?** Administra los empleados que pueden acceder al sistema del restaurante.

### Commands (Acciones)

#### CreateUser
- **Qué hace**: Registra un empleado nuevo en el sistema
- **Información**: Email, contraseña, rol, datos personales
- **Roles disponibles**: Admin (todo), Kitchen (solo cocina)
- **Validaciones**: Email único, contraseña segura

#### UpdateUser
- **Qué hace**: Modifica información de un empleado
- **Cambios típicos**: Rol, datos personales, estado (activo/inactivo)
- **Restricción**: Solo administradores pueden cambiar roles

#### DeleteUser
- **Qué hace**: Elimina acceso de un empleado
- **Uso típico**: Empleado ya no trabaja en el restaurante
- **Efecto**: No puede acceder más al sistema

#### ChangePassword
- **Qué hace**: Permite cambiar contraseña de acceso
- **Validaciones**: Contraseña actual correcta, nueva contraseña segura
- **Uso**: Seguridad o empleado olvidó su contraseña

### Queries (Consultas)

#### GetUsersByRestaurant
- **Qué hace**: Lista todos los empleados de un restaurante
- **Información**: Nombre, email, rol, estado
- **Uso**: Panel de administración de personal

#### GetUserById
- **Qué hace**: Obtiene información de un empleado específico
- **Uso**: Ver perfil, editar información

**Roles y permisos:**
- **Admin**: Puede gestionar menú, ver reportes, administrar personal, todo
- **Kitchen**: Solo puede ver pedidos de cocina, cambiar estados de órdenes

**¿Por qué es importante?**
El control de acceso asegura que cada empleado solo pueda hacer lo que corresponde a su trabajo, manteniendo la seguridad del sistema.

---

## 🔄 Cómo Se Conectan Las Features

### Flujo Típico de un Pedido
1. **Restaurants**: Configuración inicial del restaurante
2. **Users**: Creación de empleados con acceso
3. **Categories + MenuItems**: Setup del menú
4. **Tables**: Configuración de mesas con QR codes
5. **Menu**: Cliente escanea QR y ve el menú
6. **Orders**: Cliente crea pedido → CreateOrder
7. **Kitchen**: Empleado confirma pago → ConfirmOrderPayment
8. **Kitchen**: Cocina procesa → StartOrder → MarkOrderReady
9. **Reports**: Análisis de datos para mejorar

### Notificaciones en Tiempo Real
Varias features generan notificaciones que se envían via SignalR:
- **Orders**: Nueva orden → Notifica a cocina
- **Kitchen**: Estado cambia → Notifica a todas las pantallas
- **MenuItems**: Disponibilidad cambia → Actualiza menús
- **Tables**: Estado cambia → Actualiza panel de control

**¿Por qué están separadas las features?**
- **Mantenimiento fácil**: Cada funcionalidad es independiente
- **Testing simple**: Se puede probar cada feature por separado
- **Escalabilidad**: Se pueden agregar features nuevas sin afectar las existentes
- **Claridad**: Es fácil entender qué hace cada parte del sistema
