using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using middlerApp.API.IDP.DtoModels;
using middlerApp.API.IDP.Models;
using middlerApp.API.IDP.Storage.Entities;

namespace middlerApp.API.IDP.Services
{
    public class ApiScopesService : IApiScopesService
    {
        private readonly IMapper _mapper;
        private IDPDbContext DbContext { get; }
        public DataEventDispatcher EventDispatcher { get; }


        public ApiScopesService(IDPDbContext dbContext, IMapper mapper, DataEventDispatcher eventDispatcher)
        {
            _mapper = mapper;
            DbContext = dbContext;
            EventDispatcher = eventDispatcher;
        }

        public async Task<List<MScopeListDto>> GetAllApiScopeDtosAsync()
        {
            var users = await DbContext.Scopes.Where(s => s.Type == ScopeType.ApiScope).ToListAsync();
            return _mapper.Map<List<MScopeListDto>>(users);
        }

        public async Task<Scope> GetApiScopeAsync(Guid id)
        {
            return await DbContext.Scopes.WhereIsApiScope()
                .Include(u => u.Properties)
                .Include(u => u.UserClaims)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<MScopeDto> GetApiScopeDtoAsync(Guid id)
        {
            var user = await GetApiScopeAsync(id);
            return _mapper.Map<MScopeDto>(user);
        }

        public async Task CreateApiScopeAsync(MScopeDto dto)
        {
            var resource = _mapper.Map<Scope>(dto);
            resource.Type = ScopeType.ApiScope;
            await DbContext.Scopes.AddAsync(resource);
            await DbContext.SaveChangesAsync();

            EventDispatcher.DispatchCreatedEvent("IDPApiScopes", _mapper.Map<MScopeDto>(resource));
        }

        public async Task UpdateApiScopeAsync(Scope updated)
        {
            updated.Type = ScopeType.ApiScope;
            await DbContext.SaveChangesAsync();
            EventDispatcher.DispatchUpdatedEvent("IDPApiScopes", _mapper.Map<MScopeDto>(updated));
        }

        public async Task DeleteApiScopeAsync(params Guid[] id)
        {
            var resources = await DbContext.Scopes.WhereIsApiScope().Where(u => id.Contains(u.Id)).ToListAsync();
            DbContext.Scopes.RemoveRange(resources);
            await DbContext.SaveChangesAsync();
            EventDispatcher.DispatchDeletedEvent("IDPApiScopes", resources.Select(r => r.Id));
        }
    }
}
