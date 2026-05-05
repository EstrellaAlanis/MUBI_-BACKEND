namespace Mubi.Api.Models;
public class RecuperacionContrasena
{
    public int IdRecuperacionContrasena { get; set; }
    public string Correo { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public DateTime FechaSolicitud { get; set; } = DateTime.Now;
    public DateTime FechaExpiracion { get; set; }
    public bool Usado { get; set; }
}
