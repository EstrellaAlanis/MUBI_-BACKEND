using Mubi.Api.DTOs.Base; using Mubi.Api.DTOs.Create; using Mubi.Api.DTOs.Update;
namespace Mubi.Api.Services.Interfaces;
public interface IPedidoService { Task<IEnumerable<PedidoResponseDto>> GetAllAsync(); Task<PedidoResponseDto?> GetByIdAsync(int id); Task<PedidoResponseDto> CreateAsync(CreatePedidoDto dto); Task<PedidoResponseDto?> UpdateAsync(int id, UpdatePedidoDto dto); Task<bool> DeleteAsync(int id); }
