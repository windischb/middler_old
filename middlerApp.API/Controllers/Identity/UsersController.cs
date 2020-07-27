using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using middlerApp.API.Attributes;
using middlerApp.API.IDP.DtoModels;
using middlerApp.API.IDP.Models;
using middlerApp.API.IDP.Services;
using Namotion.Reflection;
using Reflectensions.ExtensionMethods;


namespace middlerApp.API.Controllers.Identity
{
    [ApiController]
    [Route("api/idp/users")]
    [AdminController]
    public class UsersController : Controller
    {
        public IUsersService UsersService { get; }
        private readonly IMapper _mapper;
        private readonly ILocalUserService _localUserService;
        private readonly IIdentityServerInteractionService _interaction;


        public UsersController(ILocalUserService localUserService,
            IIdentityServerInteractionService interaction,
            IUsersService usersService,
            IMapper mapper)
        {
            UsersService = usersService;
            _mapper = mapper;
            _localUserService = localUserService;
            _interaction = interaction;
        }

        [HttpGet]
        public async Task<ActionResult<List<MUser>>> GetAllUsers()
        {

            var users = await UsersService.GetAllUserListDtosAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MUserDto>> GetUser(string id)
        {

            if (id == "create")
            {
                return Ok(new MUserDto());
            }

            if (!Guid.TryParse(id, out var guid))
                return NotFound();

            var user = await UsersService.GetUserDtoAsync(guid);

            
            if (user == null)
                return NotFound();


            return Ok(user);


        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(MUserDto createUserDto)
        {

            var userModel = _mapper.Map<MUser>(createUserDto);
            userModel.Subject = Guid.NewGuid().ToString();

            await _localUserService.AddUserAsync(userModel);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser(MUserDto updateUserDto)
        {
            var userInDB = await UsersService.GetUserAsync(updateUserDto.Id);
            //var userModel = _mapper.Map<MUser>(updateUserDto);
            var updated = _mapper.Map(updateUserDto, userInDB);

            

            //foreach (var mRoleDto in updateUserDto.Roles)
            //{
            //    var exists = updated.UserRoles.Select(ur => ur.RoleId).Contains(mRoleDto.Id);
            //    if (!exists)
            //    {
            //        updated.UserRoles.Add(new MUserRoles(){RoleId = mRoleDto.Id});
            //    }
            //}

            await UsersService.UpdateUserAsync(updated);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {

            await UsersService.DeleteUser(id);
            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUsers([FromBody] List<Guid> ids)
        {

            await UsersService.DeleteUser(ids.ToArray());
            return NoContent();
        }


    }
}
