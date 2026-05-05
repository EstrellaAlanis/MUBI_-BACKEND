using System.ComponentModel.DataAnnotations;

namespace Mubi.Api.Models;
public class Material
{
    [Key]
    public int IdMaterial { get; set; }
    public string NombreMaterial { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public decimal StockActual { get; set; }
    public decimal StockMinimo { get; set; }
    public string UnidadMedida { get; set; } = "unidad";
    public string Estado { get; set; } = "Activo";
    public ICollection<MovimientoInventario> Movimientos { get; set; } = new List<MovimientoInventario>();
    public ICollection<ConsumoMaterial> Consumos { get; set; } = new List<ConsumoMaterial>();
}
