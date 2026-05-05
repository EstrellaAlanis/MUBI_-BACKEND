using Microsoft.AspNetCore.Mvc; using Mubi.Api.DTOs.Create; using Mubi.Api.Services.Interfaces;
namespace Mubi.Api.Controllers;
[ApiController]
[Route("api/[controller]")]
public class ContactosController : ControllerBase
{ private readonly IContactoService _service; public ContactosController(IContactoService service){_service=service;} [HttpGet] public async Task<IActionResult> Get()=>Ok(await _service.GetAllAsync()); [HttpPost] public async Task<IActionResult> Create(CreateContactoDto dto)=>Ok(await _service.CreateAsync(dto)); }
