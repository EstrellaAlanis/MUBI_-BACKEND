using Mubi.Api.DTOs.Base; using Mubi.Api.DTOs.Create;
namespace Mubi.Api.Services.Interfaces;
public interface IPagoService { Task<IEnumerable<PagoResponseDto>> GetAllAsync(); Task<PagoResponseDto?> GetByIdAsync(int id); Task<PagoResponseDto> CreateAsync(CreatePagoDto dto); Task<bool> DeleteAsync(int id); }
