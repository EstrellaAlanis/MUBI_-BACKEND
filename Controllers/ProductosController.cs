using Microsoft.AspNetCore.Mvc; using Mubi.Api.DTOs.Create; using Mubi.Api.DTOs.Update; using Mubi.Api.Services.Interfaces;
namespace Mubi.Api.Controllers;
[ApiController]
[Route("api/[controller]")]
public class ProductosController : ControllerBase
{ private readonly IProductoService _service; public ProductosController(IProductoService service){_service=service;} [HttpGet] public async Task<IActionResult> Get([FromQuery]int? idCategoria)=>Ok(await _service.GetAllAsync(idCategoria)); [HttpGet("{id:int}")] public async Task<IActionResult> GetById(int id){var data=await _service.GetByIdAsync(id); return data==null?NotFound():Ok(data);} [HttpPost] public async Task<IActionResult> Create(CreateProductoDto dto){var data=await _service.CreateAsync(dto); return CreatedAtAction(nameof(GetById), new { id = data.IdProducto }, data);} [HttpPut("{id:int}")] public async Task<IActionResult> Update(int id, UpdateProductoDto dto){var data=await _service.UpdateAsync(id,dto); return data==null?NotFound():Ok(data);} [HttpDelete("{id:int}")] public async Task<IActionResult> Delete(int id){var ok=await _service.DeleteAsync(id); return ok?NoContent():NotFound();} }
