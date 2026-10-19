# 🏪 Sistema ERP & Control de Inventariado Transaccional (Backend)

Este repositorio contiene la arquitectura **Backend de nivel empresarial** desarrollada para la gestión logística avanzada, control de existencias en tiempo real y facturación de una distribuidora comercial de abarrotes.

---

## 🛠️ Tecnologías y Arquitectura

* **Lenguaje & Framework:** C# con **.NET 8** (Web API)
* **Persistencia de Datos:** **Entity Framework Core** mediante el enfoque *Database-First*.
* **Motor de Base de Datos:** **SQL Server (SSMS)** con integridad referencial estricta.
* **Patrones de Diseño:** Implementación del patrón **Repository** asíncronos y desacoplamiento de red mediante **DTOs (Data Transfer Objects)**.

---

## 🛡️ Características Principales y Blindaje Técnico

* 📊 **Lógica Transaccional Avanzada (ACID):** Control de flujo maestro-detalle en los módulos de Compras y Ventas mediante bloques asíncronos `BeginTransactionAsync`, asegurando la consistencia e inmutabilidad de los inventarios físicos y registros automáticos en el **Kardex** (`MovimientoInventario`).
* 🔒 **Seguridad Perimetral Robusta:** Autenticación y autorización basada en una jerarquía estricta de roles (*Administrador, Vendedor, Encargado Almacén*) mediante el uso de **JWT Bearer Tokens** y hashing criptográfico de contraseñas con **BCrypt**.
* ⚙️ **Garantía Operativa:** Implementación de **Borrado Lógico** (*Soft Delete*) en tablas maestras para proteger los históricos contables y aduanas de control lógico perimetral para evitar sobreventas o stocks negativos en almacén.
* 📋 **Documentación Interactiva:** Exposición, control y testeo de endpoints documentados de forma nativa a través de **Swagger UI** y automatización de colecciones en **Postman**.

---

## 🚀 Módulos del Sistema

* 🔑 **Auth:** Control perimetral de inicio de sesión y emisión de llaves digitales.
* 👤 **Usuarios y Roles:** Administración y asignación cerrada de personal de la tienda.
* 🏢 **Almacenes y Proveedores:** Control de sucursales físicas y cadenas de suministro.
* 📦 **Productos, Categorías y Marcas:** Gestión estructurada del catálogo de abarrotes.
* 🛒 **Compras y Ventas:** Control transaccional inmutable de ingresos y salidas del inventario.
* 📊 **Inventario y Kardex:** Auditoría lineal y consolidada del stock general de la empresa.
