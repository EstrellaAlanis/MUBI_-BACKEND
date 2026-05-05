using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Mubi.Api.Data;
using Mubi.Api.DTOs.Base;
using Mubi.Api.DTOs.Create;
using Mubi.Api.Models;
using Mubi.Api.Services.Interfaces;

namespace Mubi.Api.Services.Implementations;

public class ContactoService : IContactoService
{
    private readonly MubiDbContext _context;
    private readonly IMapper _mapper;
    public ContactoService(MubiDbContext context, IMapper mapper){_context=context;_mapper=mapper;}
    public async Task<IEnumerable<ContactoResponseDto>> GetAllAsync(){var data=await _context.Contactos.OrderByDescending(x=>x.IdContacto).ToListAsync(); return _mapper.Map<IEnumerable<ContactoResponseDto>>(data);} 
    public async Task<ContactoResponseDto> CreateAsync(CreateContactoDto dto){var e=_mapper.Map<Contacto>(dto); e.FechaRegistro=DateTime.Now; e.Estado="pendiente"; if(string.IsNullOrWhiteSpace(e.Asunto)) e.Asunto="Consulta"; _context.Contactos.Add(e); await _context.SaveChangesAsync(); return _mapper.Map<ContactoResponseDto>(e);}
}
