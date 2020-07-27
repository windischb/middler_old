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

namespace middlerApp.API.IDP.Services
{
    public class UsersService : IUsersService
    {
        private readonly IMapper _mapper;
        private IDPDbContext DbContext { get; }
        public DataEventDispatcher EventDispatcher { get; }


        public UsersService(IDPDbContext dbContext, IMapper mapper, DataEventDispatcher eventDispatcher)
        {
            _mapper = mapper;
            DbContext = dbContext;
            EventDispatcher = eventDispatcher;
        }


        public async Task<List<MUserListDto>> GetAllUserListDtosAsync()
        {
            var users = await DbContext.Users.ToListAsync();
            return _mapper.Map<List<MUserListDto>>(users);
        }

        public async Task<MUser> GetUserAsync(Guid id)
        {
            return await DbContext.Users
                .Include(u => u.Claims)
                .Include(u => u.Logins)
                .Include(u => u.Secrets)
                .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<MUserDto> GetUserDtoAsync(Guid id)
        {
            var user = await GetUserAsync(id);
            return _mapper.Map<MUserDto>(user);
        }

        public async Task UpdateUserAsync(MUser userModel)
        {
            //var att = _context.Users.Attach(userModel);
            //att.State = EntityState.Modified;

            var roleIds = userModel.UserRoles.Select(ur => ur.RoleId).ToList();
            var availableRoles = DbContext.Roles.Where(r => roleIds.Contains(r.Id)).Select(r => r.Id).ToList();

            userModel.UserRoles = userModel.UserRoles.Where(ur => availableRoles.Contains(ur.RoleId)).ToList();

            await DbContext.SaveChangesAsync();
            EventDispatcher.DispatchUpdatedEvent("IDPUsers", _mapper.Map<MUserDto>(userModel));
        }

        public async Task DeleteUser(params Guid[] id)
        {
            var users = await DbContext.Users.Where(u => id.Contains(u.Id)).ToListAsync();
            DbContext.Users.RemoveRange(users);
            await DbContext.SaveChangesAsync();
            EventDispatcher.DispatchDeletedEvent("IDPUsers", users.Select(r => r.Id));
        }


    }
}
