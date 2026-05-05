namespace Mubi.Api.DTOs.Create;
public class CreateMaterialDto { public string NombreMaterial { get; set; } = string.Empty; public string? Descripcion { get; set; } public decimal StockActual { get; set; } public decimal StockMinimo { get; set; } public string UnidadMedida { get; set; } = "unidad"; public string Estado { get; set; } = "Activo"; }
