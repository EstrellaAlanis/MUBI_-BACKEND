using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Mubi.Api.Data;
using Mubi.Api.DTOs.Base;
using Mubi.Api.DTOs.Create;
using Mubi.Api.DTOs.Update;
using Mubi.Api.Models;
using Mubi.Api.Services.Interfaces;

namespace Mubi.Api.Services.Implementations;

public class ProductoService : IProductoService
{
    private readonly MubiDbContext _context;
    private readonly IMapper _mapper;
    public ProductoService(MubiDbContext context, IMapper mapper){_context=context;_mapper=mapper;}
    public async Task<IEnumerable<ProductoResponseDto>> GetAllAsync(int? idCategoria=null){var q=_context.Productos.Include(x=>x.Categoria).AsQueryable(); if(idCategoria.HasValue) q=q.Where(x=>x.IdCategoria==idCategoria.Value); var data=await q.OrderByDescending(x=>x.IdProducto).ToListAsync(); return _mapper.Map<IEnumerable<ProductoResponseDto>>(data);} 
    public async Task<ProductoResponseDto?> GetByIdAsync(int id){var e=await _context.Productos.Include(x=>x.Categoria).FirstOrDefaultAsync(x=>x.IdProducto==id); return e==null?null:_mapper.Map<ProductoResponseDto>(e);} 
    public async Task<ProductoResponseDto> CreateAsync(CreateProductoDto dto){var e=_mapper.Map<Producto>(dto); e.FechaRegistro=DateTime.Now; e.Disponibilidad=(dto.Disponibilidad ?? "disponible").Trim().ToLower(); _context.Productos.Add(e); await _context.SaveChangesAsync(); e=await _context.Productos.Include(x=>x.Categoria).FirstAsync(x=>x.IdProducto==e.IdProducto); return _mapper.Map<ProductoResponseDto>(e);} 
    public async Task<ProductoResponseDto?> UpdateAsync(int id, UpdateProductoDto dto){var e=await _context.Productos.Include(x=>x.Categoria).FirstOrDefaultAsync(x=>x.IdProducto==id); if(e==null) return null; e.Nombre=dto.Nombre; e.Descripcion=dto.Descripcion; e.Precio=dto.Precio; e.Disponibilidad=(dto.Disponibilidad ?? "disponible").Trim().ToLower(); e.IdCategoria=dto.IdCategoria; await _context.SaveChangesAsync(); e=await _context.Productos.Include(x=>x.Categoria).FirstAsync(x=>x.IdProducto==id); return _mapper.Map<ProductoResponseDto>(e);} 
    public async Task<bool> DeleteAsync(int id){var e=await _context.Productos.FindAsync(id); if(e==null) return false; _context.Productos.Remove(e); await _context.SaveChangesAsync(); return true;}
}
