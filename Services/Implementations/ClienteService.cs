using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Mubi.Api.Data;
using Mubi.Api.DTOs.Base;
using Mubi.Api.DTOs.Create;
using Mubi.Api.DTOs.Update;
using Mubi.Api.Models;
using Mubi.Api.Services.Interfaces;
using Mubi.Api.Security;
using System.Net.Http.Json;

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

        if (string.IsNullOrWhiteSpace(dto.Contrasena))
            throw new Exception("La contraseña es obligatoria.");

        if (!string.IsNullOrWhiteSpace(documento) && documento.Length != 8)
            throw new Exception("El DNI debe tener 8 dígitos.");

        if (await _context.Clientes.AnyAsync(x => x.Correo == correo))
            throw new Exception("Ya existe un cliente con ese correo.");

        if (await _context.Usuarios.AnyAsync(x => x.Correo == correo))
            throw new Exception("Ya existe un usuario con ese correo.");

        if (!string.IsNullOrWhiteSpace(documento) && await _context.Clientes.AnyAsync(x => x.DocumentoIdentidad == documento))
            throw new Exception("Ya existe un cliente con ese DNI.");

        var usuario = new Usuario
        {
            Nombre = dto.Nombres.Trim(),
            Apellido = dto.Apellidos.Trim(),
            Correo = correo,
            Contrasena = PasswordHasher.Hash(dto.Contrasena),
            Estado = "activo",
            FechaRegistro = DateTime.Now,
            IdRol = 2
        };

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        var e = new Cliente
        {
            Nombres = dto.Nombres.Trim(),
            Apellidos = dto.Apellidos.Trim(),
            Correo = correo,
            Telefono = dto.Telefono?.Trim(),
            Direccion = dto.Direccion?.Trim(),
            DocumentoIdentidad = documento,
            FechaRegistro = DateTime.Now,
            IdUsuario = usuario.IdUsuario
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

   public async Task<DniConsultaResponseDto> ConsultarDniAsync(string dni)
{
    var limpio = NormalizeDocumento(dni) ?? string.Empty;

    if (limpio.Length != 8)
        throw new Exception("Ingrese un DNI válido de 8 dígitos.");

    var token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJlbWFpbCI6ImFuZ2VsbWFuYW5pdGE5NzlAZ21haWwuY29tIn0.G4fFKJkX1pDRABGoKcI9TUIak6uaB3TUnVsHuHF-c3M";
    var url = $"https://dniruc.apisperu.com/api/v1/dni/{limpio}?token={token}";

    using var httpClient = new HttpClient();
    var response = await httpClient.GetAsync(url);

    if (!response.IsSuccessStatusCode)
        throw new Exception("No se pudo consultar el DNI en APIs Perú.");

    var json = await response.Content.ReadFromJsonAsync<ApisPeruDniResponse>();

    if (json == null || string.IsNullOrWhiteSpace(json.Nombres))
    {
        return new DniConsultaResponseDto
        {
            Dni = limpio,
            Nombres = "",
            Apellidos = "",
            Fuente = "APIs Perú",
            Encontrado = false
        };
    }

    return new DniConsultaResponseDto
    {
        Dni = limpio,
        Nombres = json.Nombres,
        Apellidos = $"{json.ApellidoPaterno} {json.ApellidoMaterno}".Trim(),
        Fuente = "APIs Perú",
        Encontrado = true
    };
}

    private static string? NormalizeDocumento(string? documento)
    {
        if (string.IsNullOrWhiteSpace(documento)) return null;
        return new string(documento.Where(char.IsDigit).ToArray());
    }
    private class ApisPeruDniResponse
{
    public string Dni { get; set; } = string.Empty;
    public string Nombres { get; set; } = string.Empty;
    public string ApellidoPaterno { get; set; } = string.Empty;
    public string ApellidoMaterno { get; set; } = string.Empty;
}
}