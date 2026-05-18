# Backend Context

## Descripción General

Backend API para **MUBI Textil Store**, sistema web de gestión y comercialización de polos sublimados/personalizados. Su responsabilidad es centralizar datos de usuarios, clientes, catálogo, pedidos, diseños, pagos, inventario y contacto; exponer endpoints REST para el frontend React; persistir información en SQL Server; guardar archivos subidos en `wwwroot/uploads`; y aplicar reglas de negocio básicas del flujo pedido → confirmación → pago → producción → entrega.

## Stack Tecnológico

- **Runtime:** .NET 8
- **Framework:** ASP.NET Core Web API
- **ORM:** Entity Framework Core 8
- **Base de datos:** SQL Server, base `MUBI_DB_FINAL`
- **Autenticación:** Login propio por correo/contraseña con BCrypt; sin JWT/session server-side real
- **Validaciones:** Validaciones manuales en servicios y controladores; DTOs simples; `[ApiController]` para validación básica del binding
- **Logs:** Logging nativo ASP.NET Core configurado en `appsettings.json`; sin logger personalizado
- **Testing:** No se observan pruebas automatizadas configuradas
- **Deploy:** No configurado; orientado a desarrollo local con `dotnet run`

## Arquitectura Backend

Arquitectura por capas simple:

`Controller → Service → DbContext/EF Core → SQL Server`

Los controladores solo exponen rutas HTTP y delegan la lógica a servicios. Los servicios contienen validaciones, normalización de datos, reglas de negocio y operaciones EF Core. `MubiDbContext` define el mapeo exacto de modelos a tablas, columnas, relaciones y triggers SQL Server. AutoMapper transforma modelos ↔ DTOs.

## Estructura de Carpetas

- `Controllers/`: endpoints REST por módulo.
- `Services/Interfaces/`: contratos de servicios.
- `Services/Implementations/`: lógica de negocio y acceso EF Core.
- `Data/`: `MubiDbContext` y configuración del modelo relacional.
- `Models/`: entidades de dominio persistidas.
- `DTOs/Base/`: DTOs de respuesta y login.
- `DTOs/Create/`: DTOs de creación.
- `DTOs/Update/`: DTOs de actualización.
- `Config/`: AutoMapper `MappingConfig`.
- `Middleware/`: manejo global de excepciones.
- `Security/`: hashing/verificación de contraseñas con BCrypt.
- `Scripts/`: scripts SQL Server de creación/parche de base de datos.
- `wwwroot/uploads/`: destino esperado para imágenes, diseños, Excel y comprobantes.

## Flujo General

1. Frontend llama a `http://localhost:5071/api/...`.
2. ASP.NET Core aplica middleware de excepciones, CORS, archivos estáticos y autorización.
3. Controller recibe DTO o archivo multipart.
4. Service valida datos, normaliza valores, aplica reglas de negocio y usa EF Core.
5. `MubiDbContext` traduce entidades a tablas SQL Server.
6. Respuesta se devuelve como DTO JSON; archivos quedan accesibles por ruta pública `/uploads/...`.

## Middlewares

- `ExceptionMiddleware`: captura excepciones no controladas y devuelve JSON `{ success, message, detail }` con status 500.
- `Swagger/SwaggerUI`: activo solo en entorno Development.
- `Cors("FrontendPolicy")`: permite frontend local `localhost:5173` y `127.0.0.1:5173`.
- `UseStaticFiles`: sirve archivos desde `wwwroot`, clave para diseños, productos, Excel y comprobantes.
- `UseAuthorization`: está habilitado, pero no hay políticas/autenticación JWT configuradas.
- `MapControllers`: registra controllers con rutas attribute-based.

## Autenticación y Autorización

- Login implementado en `UsuarioService.LoginAsync`.
- Contraseñas almacenadas con BCrypt mediante `PasswordHasher`.
- Login valida correo, contraseña y estado `activo`.
- Roles se cargan desde tabla `roles`; usuarios tienen `IdRol` y navegación `Rol`.
- No hay JWT, cookies, refresh tokens ni middleware de autenticación real.
- La protección admin/cliente se maneja principalmente desde frontend, no desde backend.
- Los endpoints CRUD quedan expuestos si se consumen directamente.

## Rutas Principales

Módulos relevantes:

- `Usuarios`: gestión de usuarios y login.
- `Clientes`: gestión de clientes, creación de usuario cliente y consulta DNI.
- `Productos`: catálogo, categorías e imagen principal por producto.
- `Pedidos`: registro de pedido, detalles, diseños, Excel de tallas y cambio de estados.
- `Pagos`: registro de pagos, comprobantes y actualización de saldo.
- `Materiales`: inventario básico y stock.
- `Contactos`: mensajes de contacto.
- `Categorias` y `Roles`: mantenimiento de datos base.

## Controladores

- Controllers son livianos y delegan al servicio correspondiente.
- `ProductosController` permite subir imagen de producto a `/uploads/productos`.
- `PedidosController` permite subir diseños a `/uploads/disenos` y Excel a `/uploads/excels`.
- `PagosController` permite subir comprobantes a `/uploads/comprobantes`.
- Controllers usan respuestas estándar `Ok`, `CreatedAtAction`, `NotFound`, `NoContent`, `BadRequest`.

## Servicios

- `UsuarioService`: CRUD de usuarios, hash BCrypt, login y validación de correo duplicado.
- `ClienteService`: CRUD de clientes, crea usuario cliente asociado, valida DNI/correo, consulta APIs Perú para DNI.
- `ProductoService`: CRUD de productos, incluye categoría e imágenes, mantiene imagen principal en `producto_imagenes`.
- `PedidoService`: crea pedidos con detalles, calcula `MontoTotal` y `SaldoPendiente`, normaliza estados, guarda historial de estados, elimina dependencias relacionadas.
- `PagoService`: registra pagos, normaliza método/tipo, recalcula saldo pendiente del pedido y cambia a `pagado` si el saldo llega a 0.
- `MaterialService`: CRUD de materiales con normalización de estado/unidad.
- `ContactoService`, `CategoriaService`, `RolService`: operaciones CRUD/listado simples.

## ORM / Acceso a Datos

- EF Core con `UseSqlServer` y `DefaultConnection` desde `appsettings.json`.
- `MubiDbContext` configura nombres de tablas/columnas manualmente para coincidir con SQL Server.
- Relaciones principales:
  - `Usuario` → `Rol` muchos a uno.
  - `Usuario` ↔ `Cliente` uno a uno opcional.
  - `Cliente` → `Pedido` uno a muchos.
  - `Pedido` → `DetallePedido`, `Pago`, `HistorialEstadoPedido`, `ConsumoMaterial` uno a muchos.
  - `Producto` → `Categoria` muchos a uno.
  - `Producto` → `ImagenProducto` uno a muchos.
- Tablas con triggers declarados en EF: `pedidos`, `pedido_detalles`, `historial_estados_pedido`, `pagos`.
- `DetallePedido.Subtotal` se marca `ValueGeneratedOnAddOrUpdate` por trigger/calculado en DB.
- Hay ajustes para evitar problemas EF/SQL Server con triggers y columnas esperadas.

## Validaciones

Estrategia actual: validaciones manuales dentro de servicios y controladores.

Validaciones importantes:

- Usuario: correo único, contraseña hasheada, estado activo para login.
- Cliente: nombres/apellidos/correo obligatorios, DNI de 8 dígitos, correo/DNI únicos, contraseña obligatoria en creación.
- Pedido: debe tener al menos un detalle; cliente y productos deben existir; cantidad mínima 1; talla/color normalizados.
- Pago: pedido debe existir; método y tipo normalizados; saldo recalculado.
- Uploads: extensiones permitidas por tipo de archivo.
- Producto/material: normalización básica de disponibilidad, estado y unidad.

No se usa FluentValidation ni DataAnnotations extensas para reglas de negocio.

## Manejo de Errores

- `ExceptionMiddleware` captura errores globales y devuelve JSON con mensaje genérico y `detail` técnico.
- Servicios lanzan `Exception` con mensajes de negocio.
- No hay clases custom error/result.
- No hay separación entre errores de validación, negocio y servidor; muchos errores de negocio terminan como HTTP 500.
- Logs dependen del logging estándar de ASP.NET Core; no hay persistencia de logs propia.

## Reglas de Negocio

- Un pedido inicia normalmente en `pendiente`.
- Estados normalizados: `pendiente`, `confirmado`, `pagado`, `en_proceso`, `entregado`, `cancelado`.
- Pedido debe tener mínimo un detalle.
- Monto total del pedido = suma de `cantidad * precioUnitario` por detalle.
- Saldo pendiente inicia igual al monto total.
- Al registrar pago, se suma lo pagado por pedido y se actualiza `SaldoPendiente`.
- Si `SaldoPendiente == 0`, el pedido pasa automáticamente a `pagado`.
- Cambios de estado generan registro en `historial_estados_pedido`.
- Clientes con pedidos no deben eliminarse desde servicio.
- Productos pueden tener imagen principal guardada en tabla `producto_imagenes`.
- Pedidos pueden tener diseño frontal/posterior y Excel de tallas/nombres/números.
- Pagos pueden tener comprobante como imagen/PDF.

## Seguridad

- Password hashing con BCrypt.
- CORS restringido al frontend local.
- No hay JWT, cookies seguras, refresh tokens ni autorización por atributo.
- No hay rate limiting.
- No hay sanitización explícita de strings más allá de trims/normalizaciones parciales.
- No hay validación de tamaño máximo de archivos.
- Uploads validan extensión, pero no validan MIME real ni escaneo de contenido.
- Token de APIs Perú para consulta DNI está hardcodeado en `ClienteService`; debería moverse a configuración/secreto.
- Endpoints administrativos no están protegidos a nivel backend.

## Dependencias Importantes

- `Microsoft.EntityFrameworkCore` / `SqlServer` / `Design`: ORM y acceso SQL Server.
- `AutoMapper.Extensions.Microsoft.DependencyInjection`: mapeo entidad-DTO.
- `BCrypt.Net-Next`: hashing de contraseñas.
- `Swashbuckle.AspNetCore`: Swagger en desarrollo.
- `System.Net.Http.Json`: consulta externa DNI.

## Variables de Entorno

No se observan variables de entorno formales. Configuración actual vía `appsettings.json`:

- `ConnectionStrings:DefaultConnection`: cadena de conexión SQL Server.
- `Logging:LogLevel`: nivel de logging ASP.NET Core.
- `AllowedHosts`: hosts permitidos por ASP.NET Core.

Valores sensibles pendientes de externalizar:

- Token APIs Perú DNI.
- Orígenes CORS si cambia el dominio frontend.
- Connection string para producción.

## Estado Actual

Funciona actualmente:

- API ASP.NET Core .NET 8 con Swagger en Development.
- Conexión SQL Server `MUBI_DB_FINAL`.
- CRUD base de roles, usuarios, clientes, categorías, productos, pedidos, pagos, materiales y contactos.
- Login con BCrypt y rol en respuesta.
- Registro de clientes con creación automática de usuario cliente.
- Consulta DNI vía APIs Perú.
- Catálogo de productos con imagen principal.
- Pedidos con detalles, diseños frontal/posterior y Excel de tallas.
- Historial de estados de pedido.
- Pagos con comprobante y recalculo de saldo.
- Archivos estáticos servidos desde `wwwroot/uploads`.
- CORS configurado para React/Vite local.

## Bugs o Problemas Conocidos

- Backend no aplica autorización real; cualquier consumidor puede llamar endpoints admin si conoce la URL.
- Errores de negocio se devuelven como 500 por el middleware global.
- Eliminación física de usuarios puede fallar si existen relaciones con clientes/pedidos.
- Algunos servicios usan `Exception` genérico en vez de respuestas controladas.
- Token de consulta DNI está hardcodeado.
- Uploads no validan tamaño ni MIME real.
- No hay tests automatizados.
- No hay migrations EF visibles; la DB depende de scripts SQL.
- Posibles conflictos por triggers SQL Server si EF no está configurado con `HasTrigger`/output clauses en nuevas tablas.
- La autorización admin/cliente depende del frontend.

## Próximos Pasos

1. Implementar autenticación real con JWT y `[Authorize]` por roles.
2. Convertir errores de negocio a respuestas 400/409 controladas.
3. Cambiar eliminación de usuarios por desactivación lógica.
4. Mover token DNI y connection string a secretos/variables de entorno.
5. Validar tamaño/MIME de archivos subidos.
6. Agregar endpoint o estado de validación de pago por admin.
7. Agregar pruebas unitarias para servicios críticos: pedidos, pagos, clientes y usuarios.
8. Documentar contrato API mínimo para frontend.
9. Revisar reglas de eliminación de productos/materiales cuando tengan relaciones.
10. Preparar configuración de despliegue y CORS para producción.