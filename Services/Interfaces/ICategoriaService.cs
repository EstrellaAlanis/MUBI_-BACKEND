using Mubi.Api.DTOs.Base; using Mubi.Api.DTOs.Create; using Mubi.Api.DTOs.Update;
namespace Mubi.Api.Services.Interfaces;
public interface ICategoriaService { Task<IEnumerable<CategoriaResponseDto>> GetAllAsync(); Task<CategoriaResponseDto?> GetByIdAsync(int id); Task<CategoriaResponseDto> CreateAsync(CreateCategoriaDto dto); Task<CategoriaResponseDto?> UpdateAsync(int id, UpdateCategoriaDto dto); Task<bool> DeleteAsync(int id); }
