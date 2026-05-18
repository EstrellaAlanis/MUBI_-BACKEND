using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Mubi.Api.Data;
using Mubi.Api.DTOs.Base;
using Mubi.Api.DTOs.Create;
using Mubi.Api.Models;
using Mubi.Api.Services.Interfaces;

namespace Mubi.Api.Services.Implementations;

public class ComprobanteVentaService : IComprobanteVentaService
{
    private readonly MubiDbContext _context;
    private readonly IMapper _mapper;

    public ComprobanteVentaService(MubiDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ComprobanteVentaResponseDto>> GetAllAsync()
    {
        var data = await _context.ComprobantesVenta
            .Include(x => x.Cliente)
            .Include(x => x.Pedido)
            .OrderByDescending(x => x.IdComprobante)
            .ToListAsync();

        return _mapper.Map<IEnumerable<ComprobanteVentaResponseDto>>(data);
    }

    public async Task<ComprobanteVentaResponseDto?> GetByIdAsync(int id)
    {
        var data = await _context.ComprobantesVenta
            .Include(x => x.Cliente)
            .Include(x => x.Pedido)
            .FirstOrDefaultAsync(x => x.IdComprobante == id);

        return data == null ? null : _mapper.Map<ComprobanteVentaResponseDto>(data);
    }

    public async Task<ComprobanteVentaResponseDto> CreateAsync(CreateComprobanteVentaDto dto)
    {
        var tipo = NormalizeTipo(dto.TipoComprobante);
        var serie = tipo == "factura" ? "F001" : "B001";

        var pedido = await _context.Pedidos
            .Include(x => x.Cliente)
            .FirstOrDefaultAsync(x => x.IdPedido == dto.IdPedido);

        if (pedido == null)
            throw new Exception("El pedido no existe.");

        if (pedido.Cliente == null)
            throw new Exception("El pedido no tiene cliente asociado.");

        if (pedido.EstadoPedido != "pagado" && pedido.SaldoPendiente > 0)
            throw new Exception("Solo se puede generar comprobante cuando el pedido está pagado o sin saldo pendiente.");

        if (await _context.ComprobantesVenta.AnyAsync(x => x.IdPedido == pedido.IdPedido && x.Estado == "emitido"))
            throw new Exception("Este pedido ya tiene un comprobante emitido.");

        if (tipo == "factura")
        {
            if (pedido.Cliente.TipoCliente != "empresa")
                throw new Exception("Solo se puede emitir factura para clientes tipo empresa.");

            if (string.IsNullOrWhiteSpace(pedido.Cliente.Ruc) || string.IsNullOrWhiteSpace(pedido.Cliente.RazonSocial))
                throw new Exception("El cliente empresa debe tener RUC y razón social.");
        }

        var ultimoNumero = await _context.ComprobantesVenta
            .Where(x => x.TipoComprobante == tipo && x.Serie == serie)
            .MaxAsync(x => (int?)x.Numero) ?? 0;

        var total = pedido.MontoTotal;
        var subtotal = Math.Round(total / 1.18m, 2);
        var igv = Math.Round(total - subtotal, 2);

        var comprobante = new ComprobanteVenta
        {
            TipoComprobante = tipo,
            Serie = serie,
            Numero = ultimoNumero + 1,
            FechaEmision = DateTime.Now,
            Subtotal = subtotal,
            Igv = igv,
            Total = total,
            Estado = "emitido",
            Observacion = dto.Observacion?.Trim(),
            IdPedido = pedido.IdPedido,
            IdCliente = pedido.IdCliente
        };

        _context.ComprobantesVenta.Add(comprobante);
        await _context.SaveChangesAsync();

        comprobante = await _context.ComprobantesVenta
            .Include(x => x.Cliente)
            .Include(x => x.Pedido)
            .FirstAsync(x => x.IdComprobante == comprobante.IdComprobante);

        return _mapper.Map<ComprobanteVentaResponseDto>(comprobante);
    }

    public async Task<bool> AnularAsync(int id)
    {
        var comprobante = await _context.ComprobantesVenta.FindAsync(id);

        if (comprobante == null)
            return false;

        comprobante.Estado = "anulado";
        comprobante.Observacion = string.IsNullOrWhiteSpace(comprobante.Observacion)
            ? "Comprobante anulado desde el sistema"
            : comprobante.Observacion;

        await _context.SaveChangesAsync();

        return true;
    }

    private static string NormalizeTipo(string? tipo)
    {
        var value = (tipo ?? "boleta").Trim().ToLower();
        return value == "factura" ? "factura" : "boleta";
    }
}