using Microsoft.AspNetCore.Mvc; using Mubi.Api.DTOs.Create; using Mubi.Api.DTOs.Update; using Mubi.Api.Services.Interfaces;
namespace Mubi.Api.Controllers;
[ApiController]
[Route("api/[controller]")]
public class PedidosController : ControllerBase
{ private readonly IPedidoService _service; public PedidosController(IPedidoService service){_service=service;} [HttpGet] public async Task<IActionResult> Get()=>Ok(await _service.GetAllAsync()); [HttpGet("{id:int}")] public async Task<IActionResult> GetById(int id){var data=await _service.GetByIdAsync(id); return data==null?NotFound():Ok(data);} [HttpPost] public async Task<IActionResult> Create(CreatePedidoDto dto){var data=await _service.CreateAsync(dto); return CreatedAtAction(nameof(GetById), new { id = data.IdPedido }, data);} [HttpPut("{id:int}")] public async Task<IActionResult> Update(int id, UpdatePedidoDto dto){var data=await _service.UpdateAsync(id,dto); return data==null?NotFound():Ok(data);} [HttpDelete("{id:int}")] public async Task<IActionResult> Delete(int id){var ok=await _service.DeleteAsync(id); return ok?NoContent():NotFound();} }
