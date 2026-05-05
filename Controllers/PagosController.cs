using Microsoft.AspNetCore.Mvc; using Mubi.Api.DTOs.Create; using Mubi.Api.Services.Interfaces;
namespace Mubi.Api.Controllers;
[ApiController]
[Route("api/[controller]")]
public class PagosController : ControllerBase
{ private readonly IPagoService _service; public PagosController(IPagoService service){_service=service;} [HttpGet] public async Task<IActionResult> Get()=>Ok(await _service.GetAllAsync()); [HttpGet("{id:int}")] public async Task<IActionResult> GetById(int id){var data=await _service.GetByIdAsync(id); return data==null?NotFound():Ok(data);} [HttpPost] public async Task<IActionResult> Create(CreatePagoDto dto){var data=await _service.CreateAsync(dto); return CreatedAtAction(nameof(GetById), new { id = data.IdPago }, data);} [HttpDelete("{id:int}")] public async Task<IActionResult> Delete(int id){var ok=await _service.DeleteAsync(id); return ok?NoContent():NotFound();} }
