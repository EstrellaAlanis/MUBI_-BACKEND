public class DetallePedidoResponseDto
{
    public int IdDetallePedido { get; set; }

    public string? Talla { get; set; }

    public string? Color { get; set; }

    public int Cantidad { get; set; }

    public string? DisenoPersonalizado { get; set; }

    public string? DescripcionDiseno { get; set; }

    public decimal PrecioUnitario { get; set; }

    public decimal Subtotal { get; set; }

    public int IdPedido { get; set; }

    public int IdProducto { get; set; }

    public string? Producto { get; set; }

    public string? RutaDisenoFrontal { get; set; }

    public string? RutaDisenoPosterior { get; set; }
}