using System.ComponentModel.DataAnnotations;

namespace Mubi.Api.Models;

public class ComprobanteVenta
{
    [Key]
    public int IdComprobante { get; set; }

    public string TipoComprobante { get; set; } = "boleta";

    public string Serie { get; set; } = "B001";

    public int Numero { get; set; }

    public DateTime FechaEmision { get; set; } = DateTime.Now;

    public decimal Subtotal { get; set; }

    public decimal Igv { get; set; }

    public decimal Total { get; set; }

    public string Estado { get; set; } = "emitido";

    public string? Observacion { get; set; }

    public int IdPedido { get; set; }

    public Pedido? Pedido { get; set; }

    public int IdCliente { get; set; }

    public Cliente? Cliente { get; set; }
}