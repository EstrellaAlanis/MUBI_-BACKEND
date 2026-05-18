namespace Mubi.Api.DTOs.Create;

public class CreateProductoDto
{
    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public decimal Precio { get; set; }

    public string Disponibilidad { get; set; } = "Disponible";

    public int IdCategoria { get; set; }

    public string? RutaImagenPrincipal { get; set; }
}