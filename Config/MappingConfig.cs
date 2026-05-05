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
        CreateMap<Producto, ProductoResponseDto>().ForMember(dest => dest.Categoria, opt => opt.MapFrom(src => src.Categoria != null ? src.Categoria.NombreCategoria : null));
        CreateMap<CreateProductoDto, Producto>();
        CreateMap<UpdateProductoDto, Producto>();
        CreateMap<Pedido, PedidoResponseDto>().ForMember(dest => dest.Cliente, opt => opt.MapFrom(src => src.Cliente != null ? $"{src.Cliente.Nombres} {src.Cliente.Apellidos}".Trim() : null));
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
