namespace Mubi.Api.Models;
public class Pedido
{
    public int IdPedido { get; set; }
    public DateTime FechaPedido { get; set; } = DateTime.Now;
    public string EstadoPedido { get; set; } = "Pendiente";
    public decimal MontoTotal { get; set; }
    public decimal SaldoPendiente { get; set; }
    public string? Observaciones { get; set; }
    public string? RutaExcelTallas { get; set; }
    public DateTime? FechaActualizacion { get; set; }
    public int IdCliente { get; set; }
    public Cliente? Cliente { get; set; }
    public ICollection<DetallePedido> Detalles { get; set; } = new List<DetallePedido>();
    public ICollection<Pago> Pagos { get; set; } = new List<Pago>();
    public ICollection<HistorialEstadoPedido> HistorialEstados { get; set; } = new List<HistorialEstadoPedido>();
    public ICollection<ConsumoMaterial> ConsumosMaterial { get; set; } = new List<ConsumoMaterial>();
}
