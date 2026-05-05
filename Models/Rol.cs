namespace Mubi.Api.Models;
public class Rol
{
    public int IdRol { get; set; }
    public string NombreRol { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
