# 🍽️ RestMg - Restaurant Management System

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4)](https://dotnet.microsoft.com/)
[![Build Status](https://img.shields.io/badge/Build-Passing-brightgreen.svg)](https://github.com/DeLxCruz/RestMg-Back)

> **Sistema de gestión de restaurantes moderno y escalable desarrollado con .NET 8 y Clean Architecture**

## 📋 Tabla de Contenidos

- [🎯 Descripción del Proyecto](#-descripción-del-proyecto)
- [✨ Características Principales](#-características-principales)
- [🏗️ Arquitectura](#️-arquitectura)
- [🛠️ Tecnologías Utilizadas](#️-tecnologías-utilizadas)
- [🚀 Inicio Rápido](#-inicio-rápido)
- [📁 Estructura del Proyecto](#-estructura-del-proyecto)
- [🔧 Configuración](#-configuración)
- [📚 Documentación Adicional](#-documentación-adicional)
- [🤝 Contribución](#-contribución)
- [👨‍💻 Autor](#-autor)

## 🎯 Descripción del Proyecto

**RestMg** es un sistema de gestión integral para restaurantes que permite administrar menús, mesas, pedidos y cocina de manera eficiente. Diseñado con arquitectura limpia y patrones de diseño modernos, ofrece una solución escalable y mantenible.

### 🎓 Contexto Académico
Este proyecto forma parte de un trabajo de grado en Ingeniería de Telecomunicaciones, demostrando la implementación de:
- Clean Architecture y principios SOLID
- Domain-Driven Design (DDD)
- Command Query Responsibility Segregation (CQRS)
- Event-Driven Architecture
- Microservicios y API RESTful

## ✨ Características Principales

### 🏪 Gestión de Restaurantes
- ✅ Onboarding de restaurantes con subdominio personalizado
- ✅ Configuración de branding (logos, colores)
- ✅ Panel de administración completo

### 🍽️ Gestión de Menús
- ✅ Creación y edición de categorías y elementos del menú
- ✅ Control de disponibilidad en tiempo real
- ✅ Subida de imágenes a Firebase Storage
- ✅ Menú público accesible por QR

### 🪑 Gestión de Mesas
- ✅ Registro y administración de mesas
- ✅ Generación automática de códigos QR
- ✅ Control de estado de mesas

### 📱 Sistema de Pedidos
- ✅ Pedidos en tiempo real sin registro de usuario
- ✅ Notificaciones automáticas a cocina
- ✅ Seguimiento de estado del pedido
- ✅ Integración con SignalR para actualizaciones en vivo

### 👨‍🍳 Módulo de Cocina
- ✅ Dashboard de órdenes activas
- ✅ Workflow de preparación (Pendiente → En preparación → Listo)
- ✅ Historial y métricas de rendimiento
- ✅ Notificaciones push en tiempo real

### 📊 Reportes y Analytics
- ✅ Dashboard con métricas en tiempo real
- ✅ Reportes de ventas y productos más vendidos
- ✅ Análisis de rendimiento de cocina

## 🏗️ Arquitectura

Este proyecto implementa **Clean Architecture** con las siguientes capas:

```
┌─────────────────────────────────────────────────┐
│                   API Layer                     │
│            (Controllers, DTOs)                  │
└─────────────────────────────────────────────────┘
┌─────────────────────────────────────────────────┐
│                Application Layer                │
│         (Use Cases, CQRS, Handlers)            │
└─────────────────────────────────────────────────┘
┌─────────────────────────────────────────────────┐
│                 Domain Layer                    │
│            (Entities, Enums)                    │
└─────────────────────────────────────────────────┘
┌─────────────────────────────────────────────────┐
│              Infrastructure Layer               │
│      (Database, External Services, Auth)       │
└─────────────────────────────────────────────────┘
```

### Principios Aplicados:
- **Separation of Concerns**: Cada capa tiene responsabilidades específicas
- **Dependency Inversion**: Las dependencias apuntan hacia adentro
- **Single Responsibility**: Una clase, una responsabilidad
- **Open/Closed**: Abierto para extensión, cerrado para modificación

## 🛠️ Tecnologías Utilizadas

### Backend (El Cerebro del Sistema)

- **Framework: .NET 8.0**  
  *¿Qué es?* Es el "motor" que hace funcionar la aplicación. Microsoft lo creó para construir aplicaciones web modernas y rápidas.  
  *¿Por qué se eligió?* Es gratuito, muy estable, rápido y tiene excelente soporte de Microsoft.

- **Arquitectura: Clean Architecture + CQRS**  
  *¿Qué es?* Es una forma de organizar el código como si fuera una casa con habitaciones específicas para cada cosa.  
  *¿Por qué es importante?* Si se necesita cambiar algo en el futuro (como cambiar de base de datos), no afecta las demás partes del sistema.

- **Base de Datos: SQL Server**  
  *¿Qué es?* Es donde se guarda toda la información (restaurantes, menús, pedidos, usuarios).  
  *¿Por qué este?* Es muy confiable, rápido para buscar información y se integra perfectamente con .NET.

- **Entity Framework Core 8.0 (ORM)**  
  *¿Qué es un ORM?* ORM significa "Object-Relational Mapping" (Mapeo Objeto-Relacional). Es como un traductor entre dos idiomas: el idioma que entienden los programadores (objetos en C#) y el idioma que entienden las bases de datos (SQL).  
  *¿Por qué es útil?* Imagina que en lugar de decir "Dame todos los pedidos del restaurante X que estén pendientes", se puede simplemente escribir `pedidos.Where(p => p.RestauranteId == X && p.Estado == Pendiente)`. Eso es lo que hace el ORM.  
  *¿Cómo funciona Entity Framework?* En lugar de escribir comandos SQL complicados como `SELECT * FROM Orders WHERE RestaurantId = 1 AND Status = 0`, se escribe código C# normal y Entity Framework se encarga de convertirlo automáticamente a SQL.  
  *Comandos de migración:*  
  - `dotnet ef migrations add NuevaCambio` → Crea un "plano" de los cambios que se quieren hacer en la base de datos  
  - `dotnet ef database update` → Aplica esos cambios a la base de datos real  
  *Ventaja principal:* Si se decide cambiar de SQL Server a MySQL mañana, solo se cambia una línea de configuración y todo sigue funcionando.

- **Autenticación: JWT (JSON Web Tokens)**  
  *¿Qué es?* Es como un "pase VIP" digital que identifica a cada usuario del sistema.  
  *¿Cómo funciona?* Cuando un usuario se conecta, se le da un token que debe presentar en cada acción, como mostrar su cédula.

- **SignalR**  
  *¿Qué es?* Es la tecnología que permite la comunicación en tiempo real.  
  *¿Para qué sirve?* Cuando llega un pedido nuevo, la cocina lo ve inmediatamente sin necesidad de actualizar la página.  
  *¿Cómo funciona?* Es como un walkie-talkie entre el sitio web del cliente y la pantalla de la cocina.

- **MediatR (Patrón Mediador)**  
  *¿Qué es?* Es como un "organizador de tareas" que recibe una solicitud y la dirige al lugar correcto.  
  *¿Por qué es útil?* Mantiene el código ordenado y facilita agregar nuevas funcionalidades sin afectar las existentes.

### Servicios Externos (Servicios de Terceros)

- **Firebase Storage**  
  *¿Qué es?* Es un servicio de Google para guardar archivos (como fotos de los platos).  
  *¿Por qué es bueno?* Es confiable, rápido, económico y se encarga de hacer las imágenes accesibles desde cualquier lugar del mundo.  
  *¿Qué devuelve?* Una URL (dirección web) donde está guardada cada imagen, que se puede mostrar en la aplicación.  
  *Ventajas:* No hay que preocuparse por el espacio en el servidor, es muy rápido para mostrar imágenes, y Google garantiza que siempre esté disponible.

### Despliegue y Hosting

- **MonsterASP.NET**  
  *¿Qué es?* Es la plataforma de hosting donde se desplegó tanto la API como la base de datos SQL Server.  
  *¿Por qué se eligió?* Permite gestionar tanto la aplicación .NET como la base de datos desde un solo panel de control, simplificando el despliegue.  
  *¿Cómo se desplegó?* Se subió el código compilado y se configuró la cadena de conexión para apuntar a la base de datos en el mismo hosting.

### ¿Por Qué No Se Implementó Testing Automatizado ni Logging Avanzado?

En este proyecto académico se priorizó demostrar la arquitectura y funcionalidades principales. **Se realizaron pruebas manuales** exhaustivas probando cada caso de uso, validaciones de entrada, manejo de errores, y flujos completos del sistema.

**Pruebas realizadas manualmente:**
- Creación y gestión de restaurantes
- Flujo completo de pedidos desde cliente hasta cocina
- Validaciones de datos en todos los formularios
- Manejo de errores y casos límite
- Integración con Firebase Storage
- Comunicación en tiempo real con SignalR
- Autenticación y autorización de usuarios

En un entorno de producción real o en futuras iteraciones del proyecto se implementarían:
- **Testing automatizado**: Unit tests, integration tests, API contract tests
- **Logging avanzado**: Registro detallado de eventos y errores
- **Telemetría**: Monitoreo de rendimiento y métricas de uso

## 🔄 Hubs de SignalR (Comunicación en Tiempo Real)

### ¿Qué son los Hubs?
Los Hubs son como "centros de comunicación" que permiten que la información fluya instantáneamente entre diferentes partes de la aplicación.

### ¿Para qué los usamos?
- **KitchenHub**: Cuando llega un pedido nuevo, la pantalla de cocina se actualiza automáticamente
- **Notificaciones**: El cliente puede ver en tiempo real cuándo su pedido está listo
- **Estados de mesa**: Si una mesa se ocupa o libera, se actualiza inmediatamente en el panel de administración

### ¿Cómo funciona?
Es como tener una radio donde la cocina, los meseros y los clientes están conectados al mismo canal. Cuando algo importante sucede, todos se enteran al instante.

## 🚀 Inicio Rápido

### Prerrequisitos
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/sql-server) o [SQL Server Express](https://www.microsoft.com/sql-server/sql-server-downloads)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) o [VS Code](https://code.visualstudio.com/)

### 1. Clonar el Repositorio
```bash
git clone https://github.com/DeLxCruz/RestMg-Back.git
cd RestMg-Back
```

### 2. Configurar User Secrets
```bash
cd API
dotnet user-secrets init
dotnet user-secrets set "Firebase:BucketName" "restmg-app"
dotnet user-secrets set "Firebase:AdminSdkPath" "path/to/firebase-credentials.json"
```

### 3. Configurar Base de Datos
```bash
# Actualizar connection string en appsettings.Development.json
dotnet ef database update --project Infrastructure --startup-project API
```

### 4. Ejecutar la Aplicación
```bash
dotnet run --project API
```

### 5. Acceder a la Aplicación
- **API**: `https://localhost:5095`
- **Swagger**: `https://localhost:5095/swagger`

## 📁 Estructura del Proyecto

```
RestMg-Back/
├── 📂 API/                     # Capa de presentación
│   ├── Controllers/            # Controladores REST API
│   ├── DTOs/                  # Data Transfer Objects
│   ├── Services/              # Servicios específicos de API
│   └── Program.cs             # Punto de entrada
├── 📂 Application/            # Capa de aplicación
│   ├── Features/              # Casos de uso organizados por feature
│   ├── Common/                # Interfaces y utilidades comunes
│   └── DependencyInjection.cs # Registro de dependencias
├── 📂 Domain/                 # Capa de dominio
│   ├── Models/                # Entidades de dominio
│   └── Enums/                 # Enumeraciones
├── 📂 Infrastructure/         # Capa de infraestructura
│   ├── Database/              # Configuración de EF Core
│   ├── Services/              # Servicios externos
│   └── Migrations/            # Migraciones de base de datos
└── 📁 docs/                   # Documentación adicional
```

## 🔧 Configuración

### Variables de Entorno

#### Desarrollo (User Secrets)
```json
{
  "Firebase:BucketName": "tu-bucket-name",
  "Firebase:AdminSdkPath": "path/to/credentials.json"
}
```

#### Producción (appsettings.json)
```json
{
  "ConnectionStrings": {
    "DeployConnection": "Server=...;Database=...;User Id=...;Password=..."
  },
  "Firebase": {
    "BucketName": "restmg-app",
    "Config": "firebase-config-json-as-string"
  },
  "JwtSettings": {
    "Secret": "your-jwt-secret",
    "Issuer": "restMg-API",
    "Audience": "restMg-Client"
  }
}
```

## 📚 Documentación Adicional

- [🏗️ Guía de Arquitectura](docs/architecture.md)
- [🔧 Guía de Desarrollo](docs/development-guide.md)
- [📡 Documentación de API](docs/api-documentation.md)
- [🚀 Guía de Deployment](docs/deployment.md)

### Documentación por Capas
- [📄 API Layer](API/README.md)
- [🔧 Application Layer](Application/README.md)
- [🏛️ Domain Layer](Domain/README.md)
- [🔌 Infrastructure Layer](Infrastructure/README.md)

## 🤝 Contribución

Este es un proyecto académico, pero las contribuciones son bienvenidas para fines educativos:

1. Fork el proyecto
2. Crea una rama feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

## 👨‍💻 Autor

**DeLxCruz**
- GitHub: [@DeLxCruz](https://github.com/DeLxCruz)
- Proyecto: [RestMg-Back](https://github.com/DeLxCruz/RestMg-Back)

---

### 🎓 Proyecto de Grado
**Universidad**: Unidades Tecnológicas de Santander  
**Carrera**: Ingeniería de Telecomunicaciones  
**Año**: 2025

---

<div align="center">
  <sub>Desarrollado con ❤️ para la gestión moderna de restaurantes</sub>
</div>
