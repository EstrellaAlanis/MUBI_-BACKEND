using System.ComponentModel.DataAnnotations;

namespace Mubi.Api.Models;

public class CodigoVerificacion
{
    [Key]
    public int IdCodigo { get; set; }
    public string Correo { get; set; } = string.Empty;
    public string Codigo { get; set; } = string.Empty;
    public string Proposito { get; set; } = "login";
    public DateTime FechaCreacion { get; set; } = DateTime.Now;
    public DateTime FechaExpiracion { get; set; }
    public bool Usado { get; set; } = false;
}
