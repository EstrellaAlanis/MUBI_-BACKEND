using System.ComponentModel.DataAnnotations;

namespace Mubi.Api.Models;

public class Cliente
{
    [Key]
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

    public DateTime FechaRegistro { get; set; } = DateTime.Now;

    public int? IdUsuario { get; set; }

    public Usuario? Usuario { get; set; }

    public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}