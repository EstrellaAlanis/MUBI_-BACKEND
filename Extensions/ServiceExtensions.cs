using Microsoft.EntityFrameworkCore;
using Mubi.Api.Data;
using Mubi.Api.Services.Implementations;
using Mubi.Api.Services.Interfaces;

namespace Mubi.Api.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddProjectServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<MubiDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IRolService, RolService>();
        services.AddScoped<IUsuarioService, UsuarioService>();
        services.AddScoped<IClienteService, ClienteService>();
        services.AddScoped<ICategoriaService, CategoriaService>();
        services.AddScoped<IProductoService, ProductoService>();
        services.AddScoped<IPedidoService, PedidoService>();
        services.AddScoped<IPagoService, PagoService>();
        services.AddScoped<IMaterialService, MaterialService>();
        services.AddScoped<IContactoService, ContactoService>();

        return services;
    }
}
