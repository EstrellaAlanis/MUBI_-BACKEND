namespace Mubi.Api.DTOs.Base;
public class MaterialResponseDto { public int IdMaterial { get; set; } public string NombreMaterial { get; set; } = string.Empty; public string? Descripcion { get; set; } public decimal StockActual { get; set; } public decimal StockMinimo { get; set; } public string UnidadMedida { get; set; } = string.Empty; public string Estado { get; set; } = string.Empty; }
