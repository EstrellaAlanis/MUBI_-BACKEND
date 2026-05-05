namespace Mubi.Api.DTOs.Create;
public class CreateContactoDto
{
    public string NombreCompleto { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string Asunto { get; set; } = "Consulta";
    public string Mensaje { get; set; } = string.Empty;
}
