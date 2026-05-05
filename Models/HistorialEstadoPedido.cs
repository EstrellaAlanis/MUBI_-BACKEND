using System.ComponentModel.DataAnnotations;

namespace Mubi.Api.Models;
public class HistorialEstadoPedido
{
    [Key]
    public int IdHistorialEstadoPedido { get; set; }
    public string EstadoAnterior { get; set; } = string.Empty;
    public string EstadoNuevo { get; set; } = string.Empty;
    public string? Observacion { get; set; }
    public DateTime FechaCambio { get; set; } = DateTime.Now;
    public int IdPedido { get; set; }
    public Pedido? Pedido { get; set; }
}
