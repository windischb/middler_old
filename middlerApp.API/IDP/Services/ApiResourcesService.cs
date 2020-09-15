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
    public class ApiResourcesService : IApiResourcesService
    {
        private readonly IMapper _mapper;
        private IDPDbContext DbContext { get; }
        public DataEventDispatcher EventDispatcher { get; }


        public ApiResourcesService(IDPDbContext dbContext, IMapper mapper, DataEventDispatcher eventDispatcher)
        {
            _mapper = mapper;
            DbContext = dbContext;
            EventDispatcher = eventDispatcher;
        }

        public async Task<List<MApiResourceListDto>> GetAllApiResourceDtosAsync()
        {
            var users = await DbContext.ApiResources.ToListAsync();
            return _mapper.Map<List<MApiResourceListDto>>(users);
        }

        public async Task<ApiResource> GetApiResourceAsync(Guid id)
        {
            return await DbContext.ApiResources
                .Include(u => u.Secrets)
                .Include(u => u.Scopes).ThenInclude(s => s.Scope).ThenInclude(s => s.UserClaims)
                .Include(u => u.UserClaims)

                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<MApiResourceDto> GetApiResourceDtoAsync(Guid id)
        {
            var user = await GetApiResourceAsync(id);
            return _mapper.Map<MApiResourceDto>(user);
        }

        public async Task CreateApiResourceAsync(MApiResourceDto dto)
        {
            var resource = _mapper.Map<ApiResource>(dto);
            await DbContext.ApiResources.AddAsync(resource);
            await DbContext.SaveChangesAsync();

            EventDispatcher.DispatchCreatedEvent("IDPApiResources", _mapper.Map<MApiResourceListDto>(resource));
        }

        public async Task UpdateApiResourceAsync(ApiResource updated)
        {
            await DbContext.SaveChangesAsync();
            EventDispatcher.DispatchUpdatedEvent("IDPApiResources", _mapper.Map<MApiResourceListDto>(updated));
        }

        public async Task DeleteApiResourceAsync(params Guid[] id)
        {
            var resources = await DbContext.ApiResources.Where(u => id.Contains(u.Id)).ToListAsync();
            DbContext.ApiResources.RemoveRange(resources);
            await DbContext.SaveChangesAsync();
            EventDispatcher.DispatchDeletedEvent("IDPApiResources", resources.Select(r => r.Id));
        }
    }
}
