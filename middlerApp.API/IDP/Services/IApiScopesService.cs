using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using middlerApp.API.IDP.DtoModels;
using middlerApp.API.IDP.Storage.Entities;

namespace middlerApp.API.IDP.Services
{
    public interface IApiScopesService
    {

        Task<List<MScopeListDto>> GetAllApiScopeDtosAsync();

        Task<Scope> GetApiScopeAsync(Guid id);

        Task<MScopeDto> GetApiScopeDtoAsync(Guid id);

        Task CreateApiScopeAsync(MScopeDto clientDto);


        Task UpdateApiScopeAsync(Scope updated);
        Task DeleteApiScopeAsync(params Guid[] id);
    }
}
