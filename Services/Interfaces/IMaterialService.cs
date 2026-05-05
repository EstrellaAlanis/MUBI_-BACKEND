using Mubi.Api.DTOs.Base; using Mubi.Api.DTOs.Create; using Mubi.Api.DTOs.Update;
namespace Mubi.Api.Services.Interfaces;
public interface IMaterialService { Task<IEnumerable<MaterialResponseDto>> GetAllAsync(); Task<MaterialResponseDto?> GetByIdAsync(int id); Task<MaterialResponseDto> CreateAsync(CreateMaterialDto dto); Task<MaterialResponseDto?> UpdateAsync(int id, UpdateMaterialDto dto); Task<bool> DeleteAsync(int id); }
