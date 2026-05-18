using Microsoft.AspNetCore.Mvc;
using Mubi.Api.DTOs.Create;
using Mubi.Api.Services.Interfaces;

namespace Mubi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PagosController : ControllerBase
{
    private readonly IPagoService _service;
    private readonly IWebHostEnvironment _env;

    public PagosController(IPagoService service, IWebHostEnvironment env)
    {
        _service = service;
        _env = env;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var data = await _service.GetByIdAsync(id);
        return data == null ? NotFound() : Ok(data);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreatePagoDto dto)
    {
        var data = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = data.IdPago }, data);
    }

    [HttpPost("upload-comprobante")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadComprobante(IFormFile archivo)
    {
        if (archivo == null || archivo.Length == 0)
            return BadRequest(new { message = "No se recibió ningún comprobante." });

        var permitidos = new[] { ".jpg", ".jpeg", ".png", ".webp", ".pdf" };
        var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();

        if (!permitidos.Contains(extension))
            return BadRequest(new { message = "Solo se permiten comprobantes JPG, PNG, WEBP o PDF." });

        var carpeta = Path.Combine(
            _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"),
            "uploads",
            "comprobantes"
        );

        if (!Directory.Exists(carpeta))
            Directory.CreateDirectory(carpeta);

        var nombreArchivo = $"{Guid.NewGuid()}{extension}";
        var rutaFisica = Path.Combine(carpeta, nombreArchivo);

        using (var stream = new FileStream(rutaFisica, FileMode.Create))
        {
            await archivo.CopyToAsync(stream);
        }

        var rutaPublica = $"/uploads/comprobantes/{nombreArchivo}";

        return Ok(new
        {
            fileName = nombreArchivo,
            ruta = rutaPublica
        });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _service.DeleteAsync(id);
        return ok ? NoContent() : NotFound();
    }
}