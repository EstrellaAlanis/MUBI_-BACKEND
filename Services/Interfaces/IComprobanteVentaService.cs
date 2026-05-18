using Mubi.Api.DTOs.Base;
using Mubi.Api.DTOs.Create;

namespace Mubi.Api.Services.Interfaces;

public interface IComprobanteVentaService
{
    Task<IEnumerable<ComprobanteVentaResponseDto>> GetAllAsync();

    Task<ComprobanteVentaResponseDto?> GetByIdAsync(int id);

    Task<ComprobanteVentaResponseDto> CreateAsync(CreateComprobanteVentaDto dto);

    Task<bool> AnularAsync(int id);
}