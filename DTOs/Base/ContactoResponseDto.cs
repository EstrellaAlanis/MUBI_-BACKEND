namespace Mubi.Api.DTOs.Base;
public class ContactoResponseDto
{
    public int IdContacto { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string Asunto { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
    public DateTime FechaRegistro { get; set; }
    public string Estado { get; set; } = string.Empty;
}
