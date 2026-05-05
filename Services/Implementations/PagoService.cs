using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Mubi.Api.Data;
using Mubi.Api.DTOs.Base;
using Mubi.Api.DTOs.Create;
using Mubi.Api.Models;
using Mubi.Api.Services.Interfaces;

namespace Mubi.Api.Services.Implementations;

public class PagoService : IPagoService
{
    private readonly MubiDbContext _context;
    private readonly IMapper _mapper;
    public PagoService(MubiDbContext context, IMapper mapper){_context=context;_mapper=mapper;}
    public async Task<IEnumerable<PagoResponseDto>> GetAllAsync(){var data=await _context.Pagos.OrderByDescending(x=>x.IdPago).ToListAsync(); return _mapper.Map<IEnumerable<PagoResponseDto>>(data);} 
    public async Task<PagoResponseDto?> GetByIdAsync(int id){var e=await _context.Pagos.FindAsync(id); return e==null?null:_mapper.Map<PagoResponseDto>(e);} 
    public async Task<PagoResponseDto> CreateAsync(CreatePagoDto dto)
    {
        var pedido=await _context.Pedidos.FirstOrDefaultAsync(x=>x.IdPedido==dto.IdPedido);
        if(pedido==null) throw new Exception("El pedido no existe.");
        var e=_mapper.Map<Pago>(dto);
        e.FechaPago=DateTime.Now;
        e.MetodoPago=NormalizeMetodo(dto.MetodoPago);
        e.TipoPago=NormalizeTipo(dto.TipoPago);
        _context.Pagos.Add(e);
        await _context.SaveChangesAsync();
        var totalPagado=await _context.Pagos.Where(x=>x.IdPedido==dto.IdPedido).SumAsync(x=>x.Monto);
        pedido.SaldoPendiente=pedido.MontoTotal-totalPagado;
        if(pedido.SaldoPendiente<0) pedido.SaldoPendiente=0;
        pedido.FechaActualizacion=DateTime.Now;
        await _context.SaveChangesAsync();
        return _mapper.Map<PagoResponseDto>(e);
    }
    public async Task<bool> DeleteAsync(int id)
    {
        var e=await _context.Pagos.FindAsync(id);
        if(e==null) return false;
        var idPedido=e.IdPedido;
        _context.Pagos.Remove(e);
        await _context.SaveChangesAsync();
        var pedido=await _context.Pedidos.FindAsync(idPedido);
        if(pedido!=null)
        {
            var totalPagado=await _context.Pagos.Where(x=>x.IdPedido==idPedido).SumAsync(x=>x.Monto);
            pedido.SaldoPendiente=pedido.MontoTotal-totalPagado;
            if(pedido.SaldoPendiente<0) pedido.SaldoPendiente=0;
            await _context.SaveChangesAsync();
        }
        return true;
    }
    private static string NormalizeMetodo(string? metodo)
    {
        var m=(metodo ?? "efectivo").Trim();
        return m.ToLower() switch { "yape" => "Yape", "plin" => "Plin", "transferencia" => "transferencia", _ => "efectivo" };
    }
    private static string NormalizeTipo(string? tipo)
    {
        var t=(tipo ?? "adelanto").Trim().ToLower().Replace(" ", "_");
        return t switch { "pago_final" => "pago_final", "pago_parcial" => "pago_parcial", _ => "adelanto" };
    }
}
