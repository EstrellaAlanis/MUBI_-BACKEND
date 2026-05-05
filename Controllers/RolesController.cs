using Microsoft.AspNetCore.Mvc;
using Mubi.Api.Services.Interfaces;
namespace Mubi.Api.Controllers;
[ApiController]
[Route("api/[controller]")]
public class RolesController : ControllerBase
{ private readonly IRolService _service; public RolesController(IRolService service){_service=service;} [HttpGet] public async Task<IActionResult> Get() => Ok(await _service.GetAllAsync()); }
