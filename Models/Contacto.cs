using System.ComponentModel.DataAnnotations;

namespace Mubi.Api.Models;
public class Contacto
{
    [Key]
    public int IdContacto { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string Asunto { get; set; } = "Consulta";
    public string Mensaje { get; set; } = string.Empty;
    public DateTime FechaRegistro { get; set; } = DateTime.Now;
    public string Estado { get; set; } = "pendiente";
}
