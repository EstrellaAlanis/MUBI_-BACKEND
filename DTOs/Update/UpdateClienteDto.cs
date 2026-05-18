namespace Mubi.Api.DTOs.Update;

public class UpdateClienteDto
{
    public string Nombres { get; set; } = string.Empty;

    public string Apellidos { get; set; } = string.Empty;

    public string Correo { get; set; } = string.Empty;

    public string? Telefono { get; set; }

    public string? Direccion { get; set; }

    public string? ReferenciaDireccion { get; set; }

    public string? DocumentoIdentidad { get; set; }

    public string TipoCliente { get; set; } = "persona";

    public string? Ruc { get; set; }

    public string? RazonSocial { get; set; }
}