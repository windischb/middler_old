using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using middlerApp.API.IDP.DtoModels;
using middlerApp.API.IDP.Storage.Entities;

namespace middlerApp.API.IDP.Services
{
    public interface IIdentityResourcesService
    {

        Task<List<MIdentityResourceListDto>> GetAllIdentityResourceDtosAsync();

        Task<IdentityResource> GetIdentityResourceAsync(Guid id);

        Task<MIdentityResourceDto> GetIdentityResourceDtoAsync(Guid id);

        Task CreateIdentityResourceAsync(MIdentityResourceDto clientDto);


        Task UpdateIdentityResourceAsync(IdentityResource updated);
        Task DeleteIdentityResourceAsync(params Guid[] id);
    }
}
