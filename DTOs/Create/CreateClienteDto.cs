namespace Mubi.Api.DTOs.Create;
public class CreateClienteDto
{
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }
    public string? DocumentoIdentidad { get; set; }
    public int? IdUsuario { get; set; }
}
