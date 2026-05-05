namespace Mubi.Api.DTOs.Base;
public class PagoResponseDto { public int IdPago { get; set; } public decimal Monto { get; set; } public DateTime FechaPago { get; set; } public string MetodoPago { get; set; } = string.Empty; public string? Comprobante { get; set; } public string TipoPago { get; set; } = string.Empty; public int IdPedido { get; set; } }
