using System.ComponentModel.DataAnnotations;

namespace Mubi.Api.Models;
public class DetallePedido
{
    [Key]
    public int IdDetallePedido { get; set; }
    public string? Talla { get; set; }
    public string? Color { get; set; }
    public int Cantidad { get; set; }
    public string? DisenoPersonalizado { get; set; }
    public string? DescripcionDiseno { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal { get; set; }
    public int IdPedido { get; set; }
    public int IdProducto { get; set; }
    public Pedido? Pedido { get; set; }
    public Producto? Producto { get; set; }
}
