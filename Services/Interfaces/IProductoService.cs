using Mubi.Api.DTOs.Base; using Mubi.Api.DTOs.Create; using Mubi.Api.DTOs.Update;
namespace Mubi.Api.Services.Interfaces;
public interface IProductoService { Task<IEnumerable<ProductoResponseDto>> GetAllAsync(int? idCategoria = null); Task<ProductoResponseDto?> GetByIdAsync(int id); Task<ProductoResponseDto> CreateAsync(CreateProductoDto dto); Task<ProductoResponseDto?> UpdateAsync(int id, UpdateProductoDto dto); Task<bool> DeleteAsync(int id); }
