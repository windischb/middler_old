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
    public class IdentityResourcesService : IIdentityResourcesService
    {
        private readonly IMapper _mapper;
        private IDPDbContext DbContext { get; }
        public DataEventDispatcher EventDispatcher { get; }


        public IdentityResourcesService(IDPDbContext dbContext, IMapper mapper, DataEventDispatcher eventDispatcher)
        {
            _mapper = mapper;
            DbContext = dbContext;
            EventDispatcher = eventDispatcher;
        }

        public async Task<List<MScopeListDto>> GetAllIdentityResourceDtosAsync()
        {
            var users = await DbContext.Scopes.WhereIsIdentityResource().ToListAsync();
            return _mapper.Map<List<MScopeListDto>>(users);
        }

        public async Task<Scope> GetIdentityResourceAsync(Guid id)
        {
            return await DbContext.Scopes.WhereIsIdentityResource()
                .Include(u => u.UserClaims)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<MScopeDto> GetIdentityResourceDtoAsync(Guid id)
        {
            var user = await GetIdentityResourceAsync(id);
            return _mapper.Map<MScopeDto>(user);
        }

        public async Task CreateIdentityResourceAsync(MScopeDto dto)
        {
            
            var resource = _mapper.Map<Scope>(dto);
            resource.Type = ScopeType.IdentityResource;
            await DbContext.Scopes.AddAsync(resource);
            await DbContext.SaveChangesAsync();

            EventDispatcher.DispatchCreatedEvent("IDPIdentityResources", _mapper.Map<MScopeDto>(resource));
        }

        public async Task UpdateIdentityResourceAsync(Scope updated)
        {
            updated.Type = ScopeType.IdentityResource;
            await DbContext.SaveChangesAsync();
            EventDispatcher.DispatchUpdatedEvent("IDPIdentityResources", _mapper.Map<MScopeDto>(updated));
        }

        public async Task DeleteIdentityResourceAsync(params Guid[] id)
        {
            var resources = await DbContext.Scopes.WhereIsIdentityResource().Where(u => id.Contains(u.Id)).ToListAsync();
            DbContext.Scopes.RemoveRange(resources);
            await DbContext.SaveChangesAsync();
            EventDispatcher.DispatchDeletedEvent("IDPIdentityResources", resources.Select(r => r.Id));
        }
    }
}
