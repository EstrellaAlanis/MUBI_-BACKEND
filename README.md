# Mubi.Api

Backend en ASP.NET Core para el proyecto **MUBI** usando una estructura parecida a **LaRicaNoche**:
- Controllers
- Data
- Models
- DTOs
- Services
- Config
- Middleware
- Security
- Scripts

## Requisitos
- .NET 8 SDK
- SQL Server
- Visual Studio Code
- Extensión C# para VS Code

## Cómo usarlo en VS Code
1. Abre la carpeta `Mubi.Api`.
2. Ejecuta `dotnet restore`.
3. Revisa la cadena de conexión en `appsettings.json`.
4. Ejecuta el script `Scripts/MUBI_DB_FINAL_SQLServer_CORREGIDA.sql` en SQL Server.
5. Luego corre `dotnet run`.
6. Entra a Swagger en `https://localhost:7071/swagger`.

## Usuario inicial
- correo: `admin@mubi.com`
- contraseña: `Admin123*`

## Nota
Este proyecto usa la organización de una API estilo LaRicaNoche, pero está adaptado al negocio MUBI.
