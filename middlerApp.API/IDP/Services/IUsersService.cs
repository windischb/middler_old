using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using middlerApp.API.IDP.DtoModels;
using middlerApp.API.IDP.Models;

namespace middlerApp.API.IDP.Services
{
    public interface IUsersService
    {
        Task<List<MUserListDto>> GetAllUserListDtosAsync();

        Task<MUser> GetUserAsync(Guid id);

        Task<MUserDto> GetUserDtoAsync(Guid id);
        
        Task DeleteUser(params Guid[] ids);
        Task UpdateUserAsync(MUser updated);
    }
}
