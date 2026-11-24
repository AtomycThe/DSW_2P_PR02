# CharacterHub – API de Gestión de Personajes (DSW_3P_PR02)

API REST en ASP.NET Core (.NET 10) con arquitectura **MVC + Service + Repository**, persistencia en **SQL Server (EF Core)**, autenticación **JWT**, **CORS** y **Swagger UI**.

## Estructura

| Carpeta | Responsabilidad |
|---|---|
| `Models/Personaje.cs` | Entidad |
| `Repositories/IPersonajeRepository.cs` | Contrato de acceso a datos |
| `Repositories/PersonajeJsonRepository.cs` | Implementación con archivo JSON (etapa 1) |
| `Repositories/PersonajeSqlRepository.cs` | Implementación con SQL Server / EF Core (etapa 2) |
| `Service/` | Reglas de negocio y validaciones |
| `Controllers/PersonajesController.cs` | Endpoints `/api/personajes` |
| `Controllers/AuthController.cs` | `POST /api/auth/login` |
| `Data/AppDbContext.cs` | DbContext de EF Core |
| `Migrations/` | Migraciones de la base de datos |

## Endpoints

| Método | Endpoint | Respuestas |
|---|---|---|
| POST | `/api/auth/login` | 200, 401 |
| GET | `/api/personajes` | 200, 401 |
| GET | `/api/personajes/{id}` | 200, 404 |
| GET | `/api/personajes/videojuego/{nombre}` | 200, 400 |
| GET | `/api/personajes/nivel/{nivel}` | 200, 400 |
| POST | `/api/personajes` | 201, 400 |
| PUT | `/api/personajes/{id}` | 200, 400, 404 |
| DELETE | `/api/personajes/{id}` | 204, 404 |

Todos los endpoints de personajes requieren `Authorization: Bearer <token>`.

## Configuración local (los secretos NO están en el repositorio)

```bash
dotnet user-secrets set "ConnectionStrings:SqlServerConexion" "Data Source=localhost,1434;Initial Catalog=DSW_3P_PR02_CharacterHub;User ID=<usuario>;Password=<password>;Encrypt=True;TrustServerCertificate=True"
dotnet user-secrets set "Jwt:Key" "$(openssl rand -base64 64)"
dotnet user-secrets set "DemoUser:Password" "<password del usuario demo>"
dotnet ef database update
dotnet run --launch-profile http
```

- URL base: `http://localhost:5217`
- Swagger UI: `http://localhost:5217/swagger`
- Usuario demo: `admin` (la contraseña se define en User Secrets)
- Orígenes CORS: `appsettings.json` → `Cors:OrigenesPermitidos` (agregar ahí el origen que indique el docente)

## Cambio de persistencia JSON → SQL Server

Componentes **sin modificar**: `PersonajesController`, `IPersonajeService` / `PersonajeService` e `IPersonajeRepository`.

Componentes **agregados o modificados**:
1. `Data/AppDbContext.cs`: DbSet `Personajes`, restricciones (longitudes, CHECK de nivel y poder) y datos iniciales.
2. `Repositories/PersonajeSqlRepository.cs`: nueva implementación de `IPersonajeRepository` usando EF Core.
3. `Program.cs`: registro de `AddDbContext` con SQL Server y cambio de la inyección `PersonajeJsonRepository` → `PersonajeSqlRepository`.
4. `Migrations/`: migración `Inicial` que crea la tabla `Personajes`.
5. `DSW_3P_PR02.csproj`: paquetes `Microsoft.EntityFrameworkCore.SqlServer` y `.Design`.

Gracias a la inyección de dependencias, el cambio de persistencia solo requirió cambiar una línea de registro en `Program.cs`.
