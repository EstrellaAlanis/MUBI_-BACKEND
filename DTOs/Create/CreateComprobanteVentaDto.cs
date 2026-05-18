namespace Mubi.Api.DTOs.Create;

public class CreateComprobanteVentaDto
{
    public string TipoComprobante { get; set; } = "boleta";

    public int IdPedido { get; set; }

    public string? Observacion { get; set; }
}   