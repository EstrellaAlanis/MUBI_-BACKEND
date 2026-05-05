using System.ComponentModel.DataAnnotations;

namespace Mubi.Api.Models;
public class Categoria
{
    [Key]
    public int IdCategoria { get; set; }
    public string NombreCategoria { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string Estado { get; set; } = "Activo";
    public ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
