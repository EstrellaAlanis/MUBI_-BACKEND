using Mubi.Api.DTOs.Base;
namespace Mubi.Api.Services.Interfaces;
public interface IRolService { Task<IEnumerable<RolResponseDto>> GetAllAsync(); }
