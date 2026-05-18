    using Microsoft.AspNetCore.Mvc;
    using Mubi.Api.DTOs.Create;
    using Mubi.Api.Services.Interfaces;

    namespace Mubi.Api.Controllers;

    [ApiController]
    [Route("api/[controller]")]
    public class ComprobantesController : ControllerBase
    {
        private readonly IComprobanteVentaService _service;

        public ComprobantesController(IComprobanteVentaService service)
        {
            _service = service;
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
        public async Task<IActionResult> Create(CreateComprobanteVentaDto dto)
        {
            var data = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = data.IdComprobante }, data);
        }

        [HttpPut("{id:int}/anular")]
        public async Task<IActionResult> Anular(int id)
        {
            var ok = await _service.AnularAsync(id);
            return ok ? NoContent() : NotFound();
        }
    }