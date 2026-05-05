namespace Mubi.Api.DTOs.Create;
public class CreatePedidoDto { public int IdCliente { get; set; } public string EstadoPedido { get; set; } = "Pendiente"; public string? Observaciones { get; set; } public List<CreateDetallePedidoDto> Detalles { get; set; } = new(); }
