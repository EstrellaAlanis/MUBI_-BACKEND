using Microsoft.AspNetCore.Mvc;
using Mubi.Api.DTOs.Create;
using Mubi.Api.DTOs.Update;
using Mubi.Api.Services.Interfaces;

namespace Mubi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PedidosController : ControllerBase
{
    private readonly IPedidoService _service;
    private readonly IWebHostEnvironment _env;

    public PedidosController(IPedidoService service, IWebHostEnvironment env)
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
    public async Task<IActionResult> Create(CreatePedidoDto dto)
    {
        var data = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = data.IdPedido }, data);
    }

    [HttpPost("subir-excel")]
    [Consumes("multipart/form-data")]

    public async Task<IActionResult> SubirExcel(IFormFile archivo)
    {
        if (archivo == null || archivo.Length == 0)
            return BadRequest(new { message = "No se recibió ningún archivo." });

        var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();

        if (extension != ".xlsx" && extension != ".xls")
            return BadRequest(new { message = "Solo se permiten archivos Excel .xlsx o .xls." });

        var carpeta = Path.Combine(
            _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"),
            "uploads",
            "excels"
        );

        if (!Directory.Exists(carpeta))
            Directory.CreateDirectory(carpeta);

        var nombreArchivo = $"{Guid.NewGuid()}{extension}";
        var rutaFisica = Path.Combine(carpeta, nombreArchivo);

        using (var stream = new FileStream(rutaFisica, FileMode.Create))
        {
            await archivo.CopyToAsync(stream);
        }

        var rutaWeb = $"/uploads/excels/{nombreArchivo}";

        return Ok(new { rutaExcelTallas = rutaWeb });
    }

    [HttpPost("upload-diseno")]
    [Consumes("multipart/form-data")]

    public async Task<IActionResult> UploadDiseno(IFormFile archivo)
    {
        if (archivo == null || archivo.Length == 0)
            return BadRequest(new { message = "No se recibió ningún archivo." });

        var permitidos = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();

        if (!permitidos.Contains(extension))
            return BadRequest(new { message = "Solo se permiten imágenes JPG, PNG o WEBP." });

        var carpeta = Path.Combine(
            _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"),
            "uploads",
            "disenos"
        );

        if (!Directory.Exists(carpeta))
            Directory.CreateDirectory(carpeta);

        var nombreArchivo = $"{Guid.NewGuid()}{extension}";
        var rutaFisica = Path.Combine(carpeta, nombreArchivo);

        using (var stream = new FileStream(rutaFisica, FileMode.Create))
        {
            await archivo.CopyToAsync(stream);
        }

        var rutaPublica = $"/uploads/disenos/{nombreArchivo}";

        return Ok(new
        {
            fileName = nombreArchivo,
            ruta = rutaPublica
        });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdatePedidoDto dto)
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
}