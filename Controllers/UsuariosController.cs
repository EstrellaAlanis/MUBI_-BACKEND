using Microsoft.AspNetCore.Mvc;
using Mubi.Api.DTOs.Auth;
using Mubi.Api.DTOs.Base;
using Mubi.Api.DTOs.Create;
using Mubi.Api.DTOs.Update;
using Mubi.Api.Services.Interfaces;

namespace Mubi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _service;

    public UsuariosController(IUsuarioService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await _service.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var data = await _service.GetByIdAsync(id);
        return data == null ? NotFound() : Ok(data);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateUsuarioDto dto)
    {
        var data = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = data.IdUsuario }, data);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateUsuarioDto dto)
    {
        var data = await _service.UpdateAsync(id, dto);
        return data == null ? NotFound() : Ok(data);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _service.DeleteAsync(id);
        return ok ? NoContent() : NotFound();
    }

    // Login tradicional. Se mantiene para pruebas/admin.
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var data = await _service.LoginAsync(dto);
        return data == null
            ? Unauthorized(new { message = "Credenciales inválidas." })
            : Ok(data);
    }

    // Paso 1: valida correo + contraseña y envía código al correo.
    [HttpPost("login/enviar-codigo")]
    public async Task<IActionResult> EnviarCodigoLogin(LoginDto dto)
    {
        var ok = await _service.EnviarCodigoLoginAsync(dto);
        return ok
            ? Ok(new { message = "Código enviado al correo." })
            : Unauthorized(new { message = "Credenciales inválidas o usuario inactivo." });
    }

    // Paso 2: verifica el código y recién devuelve la sesión del usuario.
    [HttpPost("login/verificar-codigo")]
    public async Task<IActionResult> VerificarCodigoLogin(VerificarCodigoDto dto)
    {
        var data = await _service.VerificarCodigoLoginAsync(dto);
        return data == null
            ? Unauthorized(new { message = "Código inválido o expirado." })
            : Ok(data);
    }

    // Registro paso 1: envía código al correo nuevo.
    [HttpPost("registro/enviar-codigo")]
    public async Task<IActionResult> EnviarCodigoRegistro(EnviarCodigoRegistroDto dto)
    {
        var ok = await _service.EnviarCodigoRegistroAsync(dto);
        return ok
            ? Ok(new { message = "Código de verificación enviado al correo." })
            : BadRequest(new { message = "No se pudo enviar el código." });
    }

    // Registro paso 2: valida el código antes de permitir crear la cuenta.
    [HttpPost("registro/verificar-codigo")]
    public async Task<IActionResult> VerificarCodigoRegistro(VerificarCodigoDto dto)
    {
        var ok = await _service.VerificarCodigoRegistroAsync(dto);
        return ok
            ? Ok(new { verificado = true, message = "Correo verificado correctamente." })
            : BadRequest(new { verificado = false, message = "Código inválido o expirado." });
    }
}
