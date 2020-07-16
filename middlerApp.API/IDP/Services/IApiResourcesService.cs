using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using middlerApp.API.IDP.DtoModels;
using middlerApp.API.IDP.Storage.Entities;

namespace middlerApp.API.IDP.Services
{
    public interface IApiResourcesService
    {

        Task<List<MApiResourceListDto>> GetAllApiResourceDtosAsync();

        Task<ApiResource> GetApiResourceAsync(Guid id);

        Task<MApiResourceDto> GetApiResourceDtoAsync(Guid id);

        Task CreateApiResourceAsync(MApiResourceDto clientDto);


        Task UpdateApiResourceAsync(ApiResource updated);
        Task DeleteApiResourceAsync(params Guid[] id);
    }
}
