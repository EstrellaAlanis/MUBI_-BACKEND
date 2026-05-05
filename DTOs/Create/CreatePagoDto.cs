namespace Mubi.Api.DTOs.Create;
public class CreatePagoDto { public decimal Monto { get; set; } public string MetodoPago { get; set; } = string.Empty; public string? Comprobante { get; set; } public string TipoPago { get; set; } = "Adelanto"; public int IdPedido { get; set; } }
