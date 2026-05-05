using System.ComponentModel.DataAnnotations;

namespace Mubi.Api.Models;
public class MovimientoInventario
{
    [Key]
    public int IdMovimientoInventario { get; set; }
    public string TipoMovimiento { get; set; } = "Entrada";
    public decimal Cantidad { get; set; }
    public string? Motivo { get; set; }
    public DateTime FechaMovimiento { get; set; } = DateTime.Now;
    public int IdMaterial { get; set; }
    public Material? Material { get; set; }
}
