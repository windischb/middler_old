using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using middlerApp.API.IDP.DtoModels;
using middlerApp.API.IDP.Models;

namespace middlerApp.API.IDP.Services
{
    public class RolesService : IRolesService
    {
        private readonly IMapper _mapper;
        private IDPDbContext DbContext { get; }
        public DataEventDispatcher EventDispatcher { get; }


        public RolesService(IDPDbContext dbContext, IMapper mapper, DataEventDispatcher eventDispatcher)
        {
            _mapper = mapper;
            DbContext = dbContext;
            EventDispatcher = eventDispatcher;
        }

        public async Task<List<MRoleListDto>> GetAllRoleListDtosAsync()
        {
            var roles = await DbContext.Roles.ToListAsync();
            return _mapper.Map<List<MRoleListDto>>(roles);
        }

        public async Task<MRole> GetRoleAsync(Guid id)
        {
            return await DbContext.Roles
                .Include(r => r.UserRoles).ThenInclude(ur => ur.User)
                .FirstOrDefaultAsync(c => c.Id == id);
        }


        public async Task<MRoleDto> GetRoleDtoAsync(Guid id)
        {
            var role = await GetRoleAsync(id);
            return _mapper.Map<MRoleDto>(role);
        }

        public async Task CreateRoleAsync(MRoleDto roleDto)
        {
            var role = _mapper.Map<MRole>(roleDto);
            await DbContext.Roles.AddAsync(role);
            await DbContext.SaveChangesAsync();

            EventDispatcher.DispatchCreatedEvent("IDPRoles", _mapper.Map<MRoleListDto>(role));
        }


        public async Task DeleteRole(params Guid[] id)
        {
            var roles = await DbContext.Roles.Where(u => id.Contains(u.Id)).ToListAsync();
            DbContext.Roles.RemoveRange(roles);
            await DbContext.SaveChangesAsync();

            EventDispatcher.DispatchDeletedEvent("IDPRoles", roles.Select(r => r.Id));
        }

        public async Task UpdateRoleAsync(MRole updated)
        {
            //var roleModel = _mapper.Map<MRole>(updated);
            var userIds = updated.UserRoles.Select(ur => ur.UserId).ToList();
            var availableUsers = DbContext.Users.Where(r => userIds.Contains(r.Id)).Select(r => r.Id).ToList();

            updated.UserRoles = updated.UserRoles.Where(ur => availableUsers.Contains(ur.UserId)).ToList();

            await DbContext.SaveChangesAsync();

            EventDispatcher.DispatchUpdatedEvent("IDPRoles", _mapper.Map<MRoleListDto>(updated));
        }

    }
}