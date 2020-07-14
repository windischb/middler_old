using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using middlerApp.API.IDP.DtoModels;
using middlerApp.API.IDP.Models;

namespace middlerApp.API.IDP.Services
{
    public interface IRolesService
    {
        Task<List<MRoleListDto>> GetAllRoleListDtosAsync();

        Task<MRole> GetRoleAsync(Guid id);

        Task<MRoleDto> GetRoleDtoAsync(Guid id);

        Task CreateRoleAsync(MRoleDto roleDto);

        Task DeleteRole(params Guid[] ids);
        Task UpdateRoleAsync(MRole updated);
    }
}
