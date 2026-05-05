using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Mubi.Api.Data;
using Mubi.Api.DTOs.Base;
using Mubi.Api.DTOs.Create;
using Mubi.Api.DTOs.Update;
using Mubi.Api.Models;
using Mubi.Api.Services.Interfaces;

namespace Mubi.Api.Services.Implementations;

public class ClienteService : IClienteService
{
    private readonly MubiDbContext _context;
    private readonly IMapper _mapper;

    public ClienteService(MubiDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ClienteResponseDto>> GetAllAsync()
    {
        var data = await _context.Clientes.OrderByDescending(x => x.IdCliente).ToListAsync();
        return _mapper.Map<IEnumerable<ClienteResponseDto>>(data);
    }

    public async Task<ClienteResponseDto?> GetByIdAsync(int id)
    {
        var e = await _context.Clientes.FindAsync(id);
        return e == null ? null : _mapper.Map<ClienteResponseDto>(e);
    }

    public async Task<ClienteResponseDto> CreateAsync(CreateClienteDto dto)
    {
        var correo = (dto.Correo ?? string.Empty).Trim().ToLower();
        var documento = NormalizeDocumento(dto.DocumentoIdentidad);

        if (string.IsNullOrWhiteSpace(dto.Nombres) || string.IsNullOrWhiteSpace(dto.Apellidos))
            throw new Exception("Los nombres y apellidos del cliente son obligatorios.");

        if (string.IsNullOrWhiteSpace(correo))
            throw new Exception("El correo del cliente es obligatorio.");

        if (!string.IsNullOrWhiteSpace(documento) && documento.Length != 8)
            throw new Exception("El DNI debe tener 8 dígitos.");

        if (await _context.Clientes.AnyAsync(x => x.Correo == correo))
            throw new Exception("Ya existe un cliente con ese correo.");

        if (!string.IsNullOrWhiteSpace(documento) && await _context.Clientes.AnyAsync(x => x.DocumentoIdentidad == documento))
            throw new Exception("Ya existe un cliente con ese DNI.");

        var e = new Cliente
        {
            Nombres = dto.Nombres.Trim(),
            Apellidos = dto.Apellidos.Trim(),
            Correo = correo,
            Telefono = dto.Telefono?.Trim(),
            Direccion = dto.Direccion?.Trim(),
            DocumentoIdentidad = documento,
            FechaRegistro = DateTime.Now,
            IdUsuario = dto.IdUsuario
        };

        _context.Clientes.Add(e);
        await _context.SaveChangesAsync();
        return _mapper.Map<ClienteResponseDto>(e);
    }

    public async Task<ClienteResponseDto?> UpdateAsync(int id, UpdateClienteDto dto)
    {
        var e = await _context.Clientes.FindAsync(id);
        if (e == null) return null;

        var correo = (dto.Correo ?? string.Empty).Trim().ToLower();
        var documento = NormalizeDocumento(dto.DocumentoIdentidad);

        if (string.IsNullOrWhiteSpace(dto.Nombres) || string.IsNullOrWhiteSpace(dto.Apellidos))
            throw new Exception("Los nombres y apellidos del cliente son obligatorios.");

        if (string.IsNullOrWhiteSpace(correo))
            throw new Exception("El correo del cliente es obligatorio.");

        if (!string.IsNullOrWhiteSpace(documento) && documento.Length != 8)
            throw new Exception("El DNI debe tener 8 dígitos.");

        if (await _context.Clientes.AnyAsync(x => x.IdCliente != id && x.Correo == correo))
            throw new Exception("Ya existe otro cliente con ese correo.");

        if (!string.IsNullOrWhiteSpace(documento) && await _context.Clientes.AnyAsync(x => x.IdCliente != id && x.DocumentoIdentidad == documento))
            throw new Exception("Ya existe otro cliente con ese DNI.");

        e.Nombres = dto.Nombres.Trim();
        e.Apellidos = dto.Apellidos.Trim();
        e.Correo = correo;
        e.Telefono = dto.Telefono?.Trim();
        e.Direccion = dto.Direccion?.Trim();
        e.DocumentoIdentidad = documento;

        if (e.IdUsuario.HasValue)
        {
            var usuario = await _context.Usuarios.FindAsync(e.IdUsuario.Value);
            if (usuario != null)
            {
                usuario.Nombre = e.Nombres;
                usuario.Apellido = e.Apellidos;
                usuario.Correo = e.Correo;
            }
        }

        await _context.SaveChangesAsync();
        return _mapper.Map<ClienteResponseDto>(e);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var e = await _context.Clientes.Include(x => x.Pedidos).FirstOrDefaultAsync(x => x.IdCliente == id);
        if (e == null) return false;
        if (e.Pedidos.Any()) throw new Exception("No se puede eliminar el cliente porque tiene pedidos registrados.");
        _context.Clientes.Remove(e);
        await _context.SaveChangesAsync();
        return true;
    }

    public Task<DniConsultaResponseDto> ConsultarDniAsync(string dni)
    {
        var limpio = NormalizeDocumento(dni) ?? string.Empty;
        if (limpio.Length != 8)
            throw new Exception("Ingrese un DNI válido de 8 dígitos.");

        var demo = new Dictionary<string, (string Nombres, string Apellidos)>
        {
            ["71234567"] = ("Jackeline", "Advíncula"),
            ["74561234"] = ("Rosa", "Sánchez"),
            ["70111222"] = ("Carlos", "Ruiz"),
            ["76543210"] = ("Alanis Kaley", "Estrella Coral"),
            ["12345678"] = ("Ángel Eduardo", "Mananita Asencio")
        };

        if (demo.TryGetValue(limpio, out var persona))
        {
            return Task.FromResult(new DniConsultaResponseDto
            {
                Dni = limpio,
                Nombres = persona.Nombres,
                Apellidos = persona.Apellidos,
                Fuente = "Consulta DNI preparada para integración SUNAT/RENIEC - modo demo local",
                Encontrado = true
            });
        }

        return Task.FromResult(new DniConsultaResponseDto
        {
            Dni = limpio,
            Nombres = "",
            Apellidos = "",
            Fuente = "Modo demo local: DNI no encontrado",
            Encontrado = false
        });
    }

    private static string? NormalizeDocumento(string? documento)
    {
        if (string.IsNullOrWhiteSpace(documento)) return null;
        return new string(documento.Where(char.IsDigit).ToArray());
    }
}
