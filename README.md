# Evaluación Individual: Postable - RESTful API para Gestión de Posts

### Objetivo

Desarrollar una RESTful API para una red social que permita a los usuarios interactuar con publicaciones (Posts). Esta API debe ser capaz de manejar diferentes operaciones dependiendo de si el usuario está registrado o no.

### Requerimientos Técnicos

#### Tecnologías y Herramientas

- **Lenguaje:** C#.
- **Framework:** .Net Entity Framework Core
- **Autenticación/Autorización:** JWT.
- **Base de Datos:** MS SQL Server.

#### Instrucciones de levantamiento

1. Clonar el repositorio
2. En `appsettings.json`, en `ConnectionStrings`, en `DefaultConnection`, reemplazar "REEMPLAZAME" por el nombre de su servidor de SSMS.
3. En una consola en el repositorio del proyecto, ejecutar `dotnet ef database update`
4. En una consola en el repositorio del proyecto, ejecutar `dotnet run`
5. Ir a `http://localhost:5038/swagger/index.html` en un explorador para ver la documentación de los endpoints y schemas