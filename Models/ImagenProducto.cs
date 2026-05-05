using System.ComponentModel.DataAnnotations;

namespace Mubi.Api.Models;
public class ImagenProducto
{
    [Key]
    public int IdImagenProducto { get; set; }
    public string RutaImagen { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool EsPrincipal { get; set; }
    public int IdProducto { get; set; }
    public Producto? Producto { get; set; }
}
