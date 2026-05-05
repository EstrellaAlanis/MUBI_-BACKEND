using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Mubi.Api.Data;
using Mubi.Api.DTOs.Base;
using Mubi.Api.DTOs.Create;
using Mubi.Api.DTOs.Update;
using Mubi.Api.Models;
using Mubi.Api.Services.Interfaces;

namespace Mubi.Api.Services.Implementations;

public class CategoriaService : ICategoriaService
{
    private readonly MubiDbContext _context;
    private readonly IMapper _mapper;
    public CategoriaService(MubiDbContext context, IMapper mapper){_context=context;_mapper=mapper;}
    public async Task<IEnumerable<CategoriaResponseDto>> GetAllAsync(){var data=await _context.Categorias.OrderBy(x=>x.NombreCategoria).ToListAsync(); return _mapper.Map<IEnumerable<CategoriaResponseDto>>(data);}
    public async Task<CategoriaResponseDto?> GetByIdAsync(int id){var e=await _context.Categorias.FindAsync(id); return e==null?null:_mapper.Map<CategoriaResponseDto>(e);}
    public async Task<CategoriaResponseDto> CreateAsync(CreateCategoriaDto dto){var e=_mapper.Map<Categoria>(dto); e.Estado = (dto.Estado ?? "activo").Trim().ToLower(); _context.Categorias.Add(e); await _context.SaveChangesAsync(); return _mapper.Map<CategoriaResponseDto>(e);}
    public async Task<CategoriaResponseDto?> UpdateAsync(int id, UpdateCategoriaDto dto){var e=await _context.Categorias.FindAsync(id); if(e==null) return null; e.NombreCategoria=dto.NombreCategoria; e.Descripcion=dto.Descripcion; e.Estado=(dto.Estado ?? "activo").Trim().ToLower(); await _context.SaveChangesAsync(); return _mapper.Map<CategoriaResponseDto>(e);} 
    public async Task<bool> DeleteAsync(int id){var e=await _context.Categorias.FindAsync(id); if(e==null) return false; _context.Categorias.Remove(e); await _context.SaveChangesAsync(); return true;}
}
