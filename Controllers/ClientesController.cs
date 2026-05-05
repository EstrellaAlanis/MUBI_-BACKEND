using Microsoft.AspNetCore.Mvc;
using Mubi.Api.DTOs.Create;
using Mubi.Api.DTOs.Update;
using Mubi.Api.Services.Interfaces;

namespace Mubi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    private readonly IClienteService _service;
    public ClientesController(IClienteService service) { _service = service; }

    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await _service.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var data = await _service.GetByIdAsync(id);
        return data == null ? NotFound() : Ok(data);
    }

    [HttpGet("consultar-dni/{dni}")]
    public async Task<IActionResult> ConsultarDni(string dni)
    {
        var data = await _service.ConsultarDniAsync(dni);
        return Ok(data);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateClienteDto dto)
    {
        var data = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = data.IdCliente }, data);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateClienteDto dto)
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
