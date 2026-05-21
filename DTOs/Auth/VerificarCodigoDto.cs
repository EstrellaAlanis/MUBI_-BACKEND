namespace Mubi.Api.DTOs.Auth;

public class VerificarCodigoDto
{
    public string Correo { get; set; } = string.Empty;
    public string Codigo { get; set; } = string.Empty;
}
