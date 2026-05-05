using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Mubi.Api.Data;
using Mubi.Api.DTOs.Base;
using Mubi.Api.DTOs.Create;
using Mubi.Api.DTOs.Update;
using Mubi.Api.Models;
using Mubi.Api.Services.Interfaces;

namespace Mubi.Api.Services.Implementations;

public class MaterialService : IMaterialService
{
    private readonly MubiDbContext _context;
    private readonly IMapper _mapper;
    public MaterialService(MubiDbContext context, IMapper mapper){_context=context;_mapper=mapper;}
    public async Task<IEnumerable<MaterialResponseDto>> GetAllAsync(){var data=await _context.Materiales.OrderByDescending(x=>x.IdMaterial).ToListAsync(); return _mapper.Map<IEnumerable<MaterialResponseDto>>(data);} 
    public async Task<MaterialResponseDto?> GetByIdAsync(int id){var e=await _context.Materiales.FindAsync(id); return e==null?null:_mapper.Map<MaterialResponseDto>(e);} 
    public async Task<MaterialResponseDto> CreateAsync(CreateMaterialDto dto){var e=_mapper.Map<Material>(dto); e.Estado=(dto.Estado ?? "activo").Trim().ToLower(); e.UnidadMedida=(dto.UnidadMedida ?? "unidad").Trim().ToLower(); _context.Materiales.Add(e); await _context.SaveChangesAsync(); return _mapper.Map<MaterialResponseDto>(e);} 
    public async Task<MaterialResponseDto?> UpdateAsync(int id, UpdateMaterialDto dto){var e=await _context.Materiales.FindAsync(id); if(e==null) return null; e.NombreMaterial=dto.NombreMaterial; e.Descripcion=dto.Descripcion; e.StockActual=dto.StockActual; e.StockMinimo=dto.StockMinimo; e.UnidadMedida=(dto.UnidadMedida ?? "unidad").Trim().ToLower(); e.Estado=(dto.Estado ?? "activo").Trim().ToLower(); await _context.SaveChangesAsync(); return _mapper.Map<MaterialResponseDto>(e);} 
    public async Task<bool> DeleteAsync(int id){var e=await _context.Materiales.FindAsync(id); if(e==null) return false; _context.Materiales.Remove(e); await _context.SaveChangesAsync(); return true;}
}
