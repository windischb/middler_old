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

        public async Task<List<MIdentityResourceListDto>> GetAllIdentityResourceDtosAsync()
        {
            var users = await DbContext.IdentityResources.ToListAsync();
            return _mapper.Map<List<MIdentityResourceListDto>>(users);
        }

        public async Task<IdentityResource> GetIdentityResourceAsync(Guid id)
        {
            return await DbContext.IdentityResources
                .Include(u => u.UserClaims)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<MIdentityResourceDto> GetIdentityResourceDtoAsync(Guid id)
        {
            var user = await GetIdentityResourceAsync(id);
            return _mapper.Map<MIdentityResourceDto>(user);
        }

        public async Task CreateIdentityResourceAsync(MIdentityResourceDto dto)
        {
            var resource = _mapper.Map<IdentityResource>(dto);
            await DbContext.IdentityResources.AddAsync(resource);
            await DbContext.SaveChangesAsync();

            EventDispatcher.DispatchCreatedEvent("IDPIdentityResources", _mapper.Map<MIdentityResourceListDto>(resource));
        }

        public async Task UpdateIdentityResourceAsync(IdentityResource updated)
        {
            await DbContext.SaveChangesAsync();
            EventDispatcher.DispatchUpdatedEvent("IDPIdentityResources", _mapper.Map<MIdentityResourceListDto>(updated));
        }

        public async Task DeleteIdentityResourceAsync(params Guid[] id)
        {
            var resources = await DbContext.IdentityResources.Where(u => id.Contains(u.Id)).ToListAsync();
            DbContext.IdentityResources.RemoveRange(resources);
            await DbContext.SaveChangesAsync();
            EventDispatcher.DispatchDeletedEvent("IDPIdentityResources", resources.Select(r => r.Id));
        }
    }
}
