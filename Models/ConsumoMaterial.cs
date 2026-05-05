using System.ComponentModel.DataAnnotations;

namespace Mubi.Api.Models;
public class ConsumoMaterial
{
    [Key]
    public int IdConsumoMaterial { get; set; }
    public decimal CantidadUsada { get; set; }
    public DateTime FechaRegistro { get; set; } = DateTime.Now;
    public int IdMaterial { get; set; }
    public int IdPedido { get; set; }
    public Material? Material { get; set; }
    public Pedido? Pedido { get; set; }
}
