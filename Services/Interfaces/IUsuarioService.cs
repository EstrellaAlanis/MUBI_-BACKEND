using Mubi.Api.DTOs.Base; using Mubi.Api.DTOs.Create; using Mubi.Api.DTOs.Update;
namespace Mubi.Api.Services.Interfaces;
public interface IUsuarioService { Task<IEnumerable<UsuarioResponseDto>> GetAllAsync(); Task<UsuarioResponseDto?> GetByIdAsync(int id); Task<UsuarioResponseDto> CreateAsync(CreateUsuarioDto dto); Task<UsuarioResponseDto?> UpdateAsync(int id, UpdateUsuarioDto dto); Task<bool> DeleteAsync(int id); Task<UsuarioResponseDto?> LoginAsync(LoginDto dto); }
