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
using middlerApp.API.IDP.Storage.Entities;
using Namotion.Reflection;
using Reflectensions.ExtensionMethods;


namespace middlerApp.API.Controllers.Identity
{
    [ApiController]
    [Route("api/identity/api-resources")]
    [AdminController]
    public class ApiResourceController : Controller
    {
        public IApiResourcesService ApiResourcesService { get; }
        private readonly IMapper _mapper;


        public ApiResourceController(IApiResourcesService apiResourcesService, IMapper mapper)
        {
            ApiResourcesService = apiResourcesService;
            _mapper = mapper;

        }

        [HttpGet]
        public async Task<ActionResult<List<MApiResourceListDto>>> GetList()
        {

            var resources = await ApiResourcesService.GetAllApiResourceDtosAsync();
            return Ok(resources);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MApiResourceDto>> Get(string id)
        {

            if (id == "create")
            {
                return Ok(_mapper.Map<MApiResourceDto>(new ApiResource()));
            }

            if (!Guid.TryParse(id, out var guid))
                return NotFound();

            var role = await ApiResourcesService.GetApiResourceDtoAsync(guid);

            if (role == null)
                return NotFound();

          
            return Ok(role);


        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(MApiResourceDto dto)
        {
            await ApiResourcesService.CreateApiResourceAsync(dto);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateRole(MApiResourceDto dto)
        {
            var resourceInDB = await ApiResourcesService.GetApiResourceAsync(dto.Id);
            var updated = _mapper.Map(dto, resourceInDB);

            await ApiResourcesService.UpdateApiResourceAsync(updated);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(Guid id)
        {

            await ApiResourcesService.DeleteApiResourceAsync(id);
            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteRoles([FromBody] List<Guid> ids)
        {

            await ApiResourcesService.DeleteApiResourceAsync(ids.ToArray());
            return NoContent();
        }


    }
}
