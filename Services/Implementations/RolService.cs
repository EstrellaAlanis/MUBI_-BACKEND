using AutoMapper; using Microsoft.EntityFrameworkCore; using Mubi.Api.Data; using Mubi.Api.DTOs.Base; using Mubi.Api.Services.Interfaces;
namespace Mubi.Api.Services.Implementations;
public class RolService : IRolService
{ private readonly MubiDbContext _context; private readonly IMapper _mapper; public RolService(MubiDbContext context, IMapper mapper){_context=context;_mapper=mapper;} public async Task<IEnumerable<RolResponseDto>> GetAllAsync(){var roles=await _context.Roles.OrderBy(x=>x.IdRol).ToListAsync(); return _mapper.Map<IEnumerable<RolResponseDto>>(roles);} }
