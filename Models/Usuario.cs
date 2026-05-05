namespace Mubi.Api.Models;
public class Usuario
{
    public int IdUsuario { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string Contrasena { get; set; } = string.Empty;
    public string Estado { get; set; } = "Activo";
    public DateTime FechaRegistro { get; set; } = DateTime.Now;
    public int IdRol { get; set; }
    public Rol? Rol { get; set; }
    public Cliente? Cliente { get; set; }
}
