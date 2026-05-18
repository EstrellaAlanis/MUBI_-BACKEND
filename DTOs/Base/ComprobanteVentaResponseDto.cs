namespace Mubi.Api.DTOs.Base;

public class ComprobanteVentaResponseDto
{
    public int IdComprobante { get; set; }

    public string TipoComprobante { get; set; } = string.Empty;

    public string Serie { get; set; } = string.Empty;

    public int Numero { get; set; }

    public string NumeroCompleto { get; set; } = string.Empty;

    public DateTime FechaEmision { get; set; }

    public decimal Subtotal { get; set; }

    public decimal Igv { get; set; }

    public decimal Total { get; set; }

    public string Estado { get; set; } = string.Empty;

    public string? Observacion { get; set; }

    public int IdPedido { get; set; }

    public int IdCliente { get; set; }

    public string? Cliente { get; set; }

    public string? DocumentoIdentidad { get; set; }

    public string? TipoCliente { get; set; }

    public string? Ruc { get; set; }

    public string? RazonSocial { get; set; }
}