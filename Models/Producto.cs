namespace Mubi.Api.Models;
public class Producto
{
    public int IdProducto { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public decimal Precio { get; set; }
    public string Disponibilidad { get; set; } = "Disponible";
    public DateTime FechaRegistro { get; set; } = DateTime.Now;
    public int IdCategoria { get; set; }
    public Categoria? Categoria { get; set; }
    public ICollection<ImagenProducto> Imagenes { get; set; } = new List<ImagenProducto>();
    public ICollection<DetallePedido> DetallesPedido { get; set; } = new List<DetallePedido>();
}
