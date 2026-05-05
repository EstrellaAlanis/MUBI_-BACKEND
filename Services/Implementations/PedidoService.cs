using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Mubi.Api.Data;
using Mubi.Api.DTOs.Base;
using Mubi.Api.DTOs.Create;
using Mubi.Api.DTOs.Update;
using Mubi.Api.Models;
using Mubi.Api.Services.Interfaces;

namespace Mubi.Api.Services.Implementations;

public class PedidoService : IPedidoService
{
    private readonly MubiDbContext _context;
    private readonly IMapper _mapper;
    public PedidoService(MubiDbContext context, IMapper mapper){_context=context;_mapper=mapper;}

    public async Task<IEnumerable<PedidoResponseDto>> GetAllAsync()
    {
        var data = await _context.Pedidos.Include(x=>x.Cliente).OrderByDescending(x=>x.IdPedido).ToListAsync();
        return _mapper.Map<IEnumerable<PedidoResponseDto>>(data);
    }

    public async Task<PedidoResponseDto?> GetByIdAsync(int id)
    {
        var e = await _context.Pedidos.Include(x=>x.Cliente).FirstOrDefaultAsync(x=>x.IdPedido==id);
        return e==null?null:_mapper.Map<PedidoResponseDto>(e);
    }

    public async Task<PedidoResponseDto> CreateAsync(CreatePedidoDto dto)
    {
        if(!dto.Detalles.Any()) throw new Exception("El pedido debe tener al menos un detalle.");
        if(!await _context.Clientes.AnyAsync(x => x.IdCliente == dto.IdCliente)) throw new Exception("El cliente no existe.");

        var pedido = new Pedido
        {
            IdCliente = dto.IdCliente,
            EstadoPedido = NormalizeEstado(dto.EstadoPedido),
            Observaciones = dto.Observaciones,
            FechaPedido = DateTime.Now,
            FechaActualizacion = DateTime.Now
        };

        foreach(var item in dto.Detalles)
        {
            if(!await _context.Productos.AnyAsync(x => x.IdProducto == item.IdProducto)) throw new Exception("El producto no existe.");
            pedido.Detalles.Add(new DetallePedido
            {
                Talla = string.IsNullOrWhiteSpace(item.Talla) ? "M" : item.Talla.Trim().ToUpper(),
                Color = string.IsNullOrWhiteSpace(item.Color) ? "Negro" : item.Color.Trim(),
                Cantidad = item.Cantidad <= 0 ? 1 : item.Cantidad,
                DisenoPersonalizado = item.DisenoPersonalizado,
                DescripcionDiseno = item.DescripcionDiseno,
                PrecioUnitario = item.PrecioUnitario,
                IdProducto = item.IdProducto
            });
        }

        pedido.MontoTotal = pedido.Detalles.Sum(x=>x.Cantidad * x.PrecioUnitario);
        pedido.SaldoPendiente = pedido.MontoTotal;
        pedido.HistorialEstados.Add(new HistorialEstadoPedido
        {
            EstadoAnterior = "sin_estado",
            EstadoNuevo = pedido.EstadoPedido,
            Observacion = "Creación del pedido",
            FechaCambio = DateTime.Now
        });

        _context.Pedidos.Add(pedido);
        await _context.SaveChangesAsync();
        pedido = await _context.Pedidos.Include(x=>x.Cliente).FirstAsync(x=>x.IdPedido==pedido.IdPedido);
        return _mapper.Map<PedidoResponseDto>(pedido);
    }

    public async Task<PedidoResponseDto?> UpdateAsync(int id, UpdatePedidoDto dto)
    {
        var e = await _context.Pedidos.Include(x=>x.Cliente).FirstOrDefaultAsync(x=>x.IdPedido==id);
        if(e==null) return null;
        var anterior = e.EstadoPedido;
        e.EstadoPedido = NormalizeEstado(dto.EstadoPedido);
        e.Observaciones = dto.Observaciones;
        e.FechaActualizacion = DateTime.Now;
        if(!string.Equals(anterior,e.EstadoPedido,StringComparison.OrdinalIgnoreCase))
        {
            _context.HistorialEstadosPedido.Add(new HistorialEstadoPedido
            {
                IdPedido=e.IdPedido,
                EstadoAnterior=anterior,
                EstadoNuevo=e.EstadoPedido,
                Observacion="Cambio de estado desde la API",
                FechaCambio=DateTime.Now
            });
        }
        await _context.SaveChangesAsync();
        return _mapper.Map<PedidoResponseDto>(e);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var e = await _context.Pedidos.Include(x => x.Detalles).Include(x => x.Pagos).Include(x => x.HistorialEstados).FirstOrDefaultAsync(x => x.IdPedido == id);
        if(e==null) return false;
        _context.DetallesPedido.RemoveRange(e.Detalles);
        _context.Pagos.RemoveRange(e.Pagos);
        _context.HistorialEstadosPedido.RemoveRange(e.HistorialEstados);
        _context.Pedidos.Remove(e);
        await _context.SaveChangesAsync();
        return true;
    }

    private static string NormalizeEstado(string? estado)
    {
        var value = (estado ?? "pendiente").Trim().ToLower().Replace(" ", "_");
        return value switch
        {
            "confirmado" => "confirmado",
            "en_proceso" => "en_proceso",
            "entregado" => "entregado",
            "cancelado" => "cancelado",
            _ => "pendiente"
        };
    }
}
