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

        Task<List<MScopeListDto>> GetAllIdentityResourceDtosAsync();

        Task<Scope> GetIdentityResourceAsync(Guid id);

        Task<MScopeDto> GetIdentityResourceDtoAsync(Guid id);

        Task CreateIdentityResourceAsync(MScopeDto clientDto);


        Task UpdateIdentityResourceAsync(Scope updated);
        Task DeleteIdentityResourceAsync(params Guid[] id);
    }
}
