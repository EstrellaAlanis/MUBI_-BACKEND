namespace Mubi.Api.DTOs.Base;

public class ClienteResponseDto
{
    public int IdCliente { get; set; }

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

    public DateTime FechaRegistro { get; set; }

    public int? IdUsuario { get; set; }
}