using System.Security.Cryptography;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Mubi.Api.Data;
using Mubi.Api.DTOs.Auth;
using Mubi.Api.DTOs.Base;
using Mubi.Api.DTOs.Create;
using Mubi.Api.DTOs.Update;
using Mubi.Api.Models;
using Mubi.Api.Security;
using Mubi.Api.Services.Interfaces;

namespace Mubi.Api.Services.Implementations;

public class UsuarioService : IUsuarioService
{
    private readonly MubiDbContext _context;
    private readonly IMapper _mapper;
    private readonly IEmailService _emailService;

    public UsuarioService(MubiDbContext context, IMapper mapper, IEmailService emailService)
    {
        _context = context;
        _mapper = mapper;
        _emailService = emailService;
    }

    public async Task<IEnumerable<UsuarioResponseDto>> GetAllAsync()
    {
        var data = await _context.Usuarios
            .Include(x => x.Rol)
            .OrderByDescending(x => x.IdUsuario)
            .ToListAsync();

        return _mapper.Map<IEnumerable<UsuarioResponseDto>>(data);
    }

    public async Task<UsuarioResponseDto?> GetByIdAsync(int id)
    {
        var e = await _context.Usuarios
            .Include(x => x.Rol)
            .FirstOrDefaultAsync(x => x.IdUsuario == id);

        return e == null ? null : _mapper.Map<UsuarioResponseDto>(e);
    }

    public async Task<UsuarioResponseDto> CreateAsync(CreateUsuarioDto dto)
    {
        var correo = NormalizeEmail(dto.Correo);

        if (await _context.Usuarios.AnyAsync(x => x.Correo.ToLower() == correo))
            throw new Exception("Ya existe un usuario con ese correo.");

        var e = _mapper.Map<Usuario>(dto);
        e.Correo = correo;
        e.Contrasena = PasswordHasher.Hash(dto.Contrasena);
        e.FechaRegistro = DateTime.Now;
        e.Estado = "activo";

        _context.Usuarios.Add(e);
        await _context.SaveChangesAsync();

        e = await _context.Usuarios
            .Include(x => x.Rol)
            .FirstAsync(x => x.IdUsuario == e.IdUsuario);

        return _mapper.Map<UsuarioResponseDto>(e);
    }

    public async Task<UsuarioResponseDto?> UpdateAsync(int id, UpdateUsuarioDto dto)
    {
        var e = await _context.Usuarios
            .Include(x => x.Rol)
            .FirstOrDefaultAsync(x => x.IdUsuario == id);

        if (e == null) return null;

        e.Nombre = dto.Nombre;
        e.Apellido = dto.Apellido;
        e.Estado = (dto.Estado ?? "activo").Trim().ToLower();
        e.IdRol = dto.IdRol;

        await _context.SaveChangesAsync();

        e = await _context.Usuarios
            .Include(x => x.Rol)
            .FirstAsync(x => x.IdUsuario == id);

        return _mapper.Map<UsuarioResponseDto>(e);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var e = await _context.Usuarios.FindAsync(id);
        if (e == null) return false;

        _context.Usuarios.Remove(e);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<UsuarioResponseDto?> LoginAsync(LoginDto dto)
    {
        var correo = NormalizeEmail(dto.Correo);

        var e = await _context.Usuarios
            .Include(x => x.Rol)
            .FirstOrDefaultAsync(x => x.Correo.ToLower() == correo);

        if (e == null ||
            !PasswordHasher.Verify(dto.Contrasena, e.Contrasena) ||
            !string.Equals(e.Estado, "activo", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        return _mapper.Map<UsuarioResponseDto>(e);
    }

    public async Task<bool> EnviarCodigoLoginAsync(LoginDto dto)
    {
        var correo = NormalizeEmail(dto.Correo);

        var usuario = await _context.Usuarios
            .Include(x => x.Rol)
            .FirstOrDefaultAsync(x => x.Correo.ToLower() == correo);

        if (usuario == null ||
            !PasswordHasher.Verify(dto.Contrasena, usuario.Contrasena) ||
            !string.Equals(usuario.Estado, "activo", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var codigo = GenerateCode();
        await GuardarCodigoAsync(correo, codigo, "login");
        await _emailService.SendVerificationCodeAsync(correo, codigo, "login");

        return true;
    }

    public async Task<UsuarioResponseDto?> VerificarCodigoLoginAsync(VerificarCodigoDto dto)
    {
        var correo = NormalizeEmail(dto.Correo);
        var ok = await ValidarCodigoAsync(correo, dto.Codigo, "login");

        if (!ok) return null;

        var usuario = await _context.Usuarios
            .Include(x => x.Rol)
            .FirstOrDefaultAsync(x => x.Correo.ToLower() == correo);

        return usuario == null ? null : _mapper.Map<UsuarioResponseDto>(usuario);
    }

    public async Task<bool> EnviarCodigoRegistroAsync(EnviarCodigoRegistroDto dto)
    {
        var correo = NormalizeEmail(dto.Correo);

        if (string.IsNullOrWhiteSpace(correo))
            throw new Exception("El correo es obligatorio.");

        var existeUsuario = await _context.Usuarios.AnyAsync(x => x.Correo.ToLower() == correo);
        if (existeUsuario)
            throw new Exception("Este correo ya tiene una cuenta registrada. Inicia sesión.");

        var codigo = GenerateCode();
        await GuardarCodigoAsync(correo, codigo, "registro");
        await _emailService.SendVerificationCodeAsync(correo, codigo, "registro");

        return true;
    }

    public async Task<bool> VerificarCodigoRegistroAsync(VerificarCodigoDto dto)
    {
        var correo = NormalizeEmail(dto.Correo);
        return await ValidarCodigoAsync(correo, dto.Codigo, "registro");
    }

    public async Task<bool> EnviarCodigoGoogleAsync(EnviarCodigoRegistroDto dto)
    {
        var correo = NormalizeEmail(dto.Correo);

        if (string.IsNullOrWhiteSpace(correo))
            throw new Exception("El correo de Google es obligatorio.");

        var codigo = GenerateCode();
        await GuardarCodigoAsync(correo, codigo, "google");
        await _emailService.SendVerificationCodeAsync(correo, codigo, "google");

        return true;
    }

    public async Task<UsuarioResponseDto?> VerificarCodigoGoogleAsync(VerificarCodigoDto dto)
    {
        var correo = NormalizeEmail(dto.Correo);
        var ok = await ValidarCodigoAsync(correo, dto.Codigo, "google");

        if (!ok) return null;

        var usuario = await _context.Usuarios
            .Include(x => x.Rol)
            .FirstOrDefaultAsync(x => x.Correo.ToLower() == correo);

        if (usuario == null)
        {
            usuario = await CrearUsuarioClienteGoogleAsync(correo);
        }

        return _mapper.Map<UsuarioResponseDto>(usuario);
    }

    private async Task<Usuario> CrearUsuarioClienteGoogleAsync(string correo)
    {
        var nombreBase = correo.Split('@')[0];
        var nombreLegible = string.IsNullOrWhiteSpace(nombreBase)
            ? "Cliente Google"
            : char.ToUpper(nombreBase[0]) + nombreBase[1..];

        var usuario = new Usuario
        {
            Nombre = nombreLegible,
            Apellido = "",
            Correo = correo,
            Contrasena = PasswordHasher.Hash($"Google-{Guid.NewGuid():N}*"),
            Estado = "activo",
            FechaRegistro = DateTime.Now,
            IdRol = 2
        };

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        var existeCliente = await _context.Clientes.AnyAsync(x => x.Correo.ToLower() == correo);

        if (!existeCliente)
        {
            var cliente = new Cliente
            {
                Nombres = nombreLegible,
                Apellidos = "",
                Correo = correo,
                Telefono = "",
                Direccion = "",
                ReferenciaDireccion = "",
                TipoCliente = "persona",
                Ruc = null,
                RazonSocial = null,
                DocumentoIdentidad = "",
                FechaRegistro = DateTime.Now,
                IdUsuario = usuario.IdUsuario
            };

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();
        }

        usuario = await _context.Usuarios
            .Include(x => x.Rol)
            .FirstAsync(x => x.IdUsuario == usuario.IdUsuario);

        return usuario;
    }

    private async Task GuardarCodigoAsync(string correo, string codigo, string proposito)
    {
        var anteriores = await _context.CodigosVerificacion
            .Where(x => x.Correo == correo && x.Proposito == proposito && !x.Usado)
            .ToListAsync();

        foreach (var item in anteriores)
            item.Usado = true;

        _context.CodigosVerificacion.Add(new CodigoVerificacion
        {
            Correo = correo,
            Codigo = codigo,
            Proposito = proposito,
            FechaCreacion = DateTime.Now,
            FechaExpiracion = DateTime.Now.AddMinutes(10),
            Usado = false
        });

        await _context.SaveChangesAsync();
    }

    private async Task<bool> ValidarCodigoAsync(string correo, string codigo, string proposito)
    {
        var codigoLimpio = (codigo ?? string.Empty).Trim();

        var registro = await _context.CodigosVerificacion
            .Where(x => x.Correo == correo &&
                        x.Codigo == codigoLimpio &&
                        x.Proposito == proposito &&
                        !x.Usado)
            .OrderByDescending(x => x.IdCodigo)
            .FirstOrDefaultAsync();

        if (registro == null) return false;

        if (registro.FechaExpiracion < DateTime.Now)
            return false;

        registro.Usado = true;
        await _context.SaveChangesAsync();

        return true;
    }

    private static string GenerateCode()
    {
        return RandomNumberGenerator.GetInt32(100000, 999999).ToString();
    }

    private static string NormalizeEmail(string? email)
    {
        return (email ?? string.Empty).Trim().ToLower();
    }
}
