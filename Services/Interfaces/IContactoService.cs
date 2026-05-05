using Mubi.Api.DTOs.Base; using Mubi.Api.DTOs.Create;
namespace Mubi.Api.Services.Interfaces;
public interface IContactoService { Task<IEnumerable<ContactoResponseDto>> GetAllAsync(); Task<ContactoResponseDto> CreateAsync(CreateContactoDto dto); }
