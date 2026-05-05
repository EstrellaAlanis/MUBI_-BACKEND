using System.ComponentModel.DataAnnotations;

namespace Mubi.Api.Models;
public class Pago
{
    [Key]
    public int IdPago { get; set; }
    public decimal Monto { get; set; }
    public DateTime FechaPago { get; set; } = DateTime.Now;
    public string MetodoPago { get; set; } = string.Empty;
    public string? Comprobante { get; set; }
    public string TipoPago { get; set; } = "Adelanto";
    public int IdPedido { get; set; }
    public Pedido? Pedido { get; set; }
}
