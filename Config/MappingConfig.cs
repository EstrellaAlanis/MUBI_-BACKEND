using AutoMapper;
using Mubi.Api.DTOs.Base;
using Mubi.Api.DTOs.Create;
using Mubi.Api.DTOs.Update;
using Mubi.Api.Models;

namespace Mubi.Api.Config;

public class MappingConfig : Profile
{
    public MappingConfig()
    {
        CreateMap<Rol, RolResponseDto>().ReverseMap();
        CreateMap<Usuario, UsuarioResponseDto>().ForMember(dest => dest.Rol, opt => opt.MapFrom(src => src.Rol != null ? src.Rol.NombreRol : null));
        CreateMap<CreateUsuarioDto, Usuario>();
        CreateMap<Cliente, ClienteResponseDto>().ReverseMap();
        CreateMap<CreateClienteDto, Cliente>();
        CreateMap<UpdateClienteDto, Cliente>();
        CreateMap<Categoria, CategoriaResponseDto>().ReverseMap();
        CreateMap<CreateCategoriaDto, Categoria>();
        CreateMap<UpdateCategoriaDto, Categoria>();
        CreateMap<Producto, ProductoResponseDto>()
        .ForMember(dest => dest.Categoria,
            opt => opt.MapFrom(src => src.Categoria != null ? src.Categoria.NombreCategoria : null))
        .ForMember(dest => dest.RutaImagenPrincipal,
            opt => opt.MapFrom(src =>
                src.Imagenes != null && src.Imagenes.Any()
                    ? src.Imagenes
                        .OrderByDescending(i => i.EsPrincipal)
                        .ThenBy(i => i.IdImagenProducto)
                        .Select(i => i.RutaImagen)
                        .FirstOrDefault()
                    : null));        CreateMap<CreateProductoDto, Producto>();
        CreateMap<UpdateProductoDto, Producto>();
        CreateMap<DetallePedido, DetallePedidoResponseDto>()
    .ForMember(dest => dest.Producto,
        opt => opt.MapFrom(src => src.Producto != null ? src.Producto.Nombre : null));

CreateMap<Pedido, PedidoResponseDto>()
    .ForMember(dest => dest.Cliente,
        opt => opt.MapFrom(src => src.Cliente != null
            ? $"{src.Cliente.Nombres} {src.Cliente.Apellidos}".Trim()
            : null))
    .ForMember(dest => dest.Detalles,
        opt => opt.MapFrom(src => src.Detalles));
        CreateMap<CreatePedidoDto, Pedido>(); 
        CreateMap<Pago, PagoResponseDto>().ReverseMap();
        CreateMap<CreatePagoDto, Pago>();
        CreateMap<Material, MaterialResponseDto>().ReverseMap();
        CreateMap<CreateMaterialDto, Material>();
        CreateMap<UpdateMaterialDto, Material>();
        CreateMap<Contacto, ContactoResponseDto>().ReverseMap();
        CreateMap<CreateContactoDto, Contacto>();
    }
}
