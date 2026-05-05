using Mubi.Api.DTOs.Base;
using Mubi.Api.DTOs.Create;
using Mubi.Api.DTOs.Update;

namespace Mubi.Api.Services.Interfaces;
public interface IClienteService
{
    Task<IEnumerable<ClienteResponseDto>> GetAllAsync();
    Task<ClienteResponseDto?> GetByIdAsync(int id);
    Task<ClienteResponseDto> CreateAsync(CreateClienteDto dto);
    Task<ClienteResponseDto?> UpdateAsync(int id, UpdateClienteDto dto);
    Task<bool> DeleteAsync(int id);
    Task<DniConsultaResponseDto> ConsultarDniAsync(string dni);
}
