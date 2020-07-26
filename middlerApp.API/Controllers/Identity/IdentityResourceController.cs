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
    [Route("api/identity/identity-resources")]
    [AdminController]
    public class IdentityResourceController : Controller
    {
        public IIdentityResourcesService IdentityResourcesService { get; }
        private readonly IMapper _mapper;


        public IdentityResourceController(IIdentityResourcesService apiResourcesService, IMapper mapper)
        {
            IdentityResourcesService = apiResourcesService;
            _mapper = mapper;

        }

        [HttpGet]
        public async Task<ActionResult<List<MIdentityResourceListDto>>> GetList()
        {

            var resources = await IdentityResourcesService.GetAllIdentityResourceDtosAsync();
            return Ok(resources);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MIdentityResourceDto>> Get(string id)
        {

            if (id == "create")
            {
                return Ok(_mapper.Map<MIdentityResourceDto>(new IdentityResource()));
            }

            if (!Guid.TryParse(id, out var guid))
                return NotFound();

            var apiResource = await IdentityResourcesService.GetIdentityResourceDtoAsync(guid);

            if (apiResource == null)
                return NotFound();

          
            return Ok(apiResource);


        }

        [HttpPost]
        public async Task<IActionResult> CreateIdentityResource(MIdentityResourceDto dto)
        {
            await IdentityResourcesService.CreateIdentityResourceAsync(dto);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateIdentityResource(MIdentityResourceDto dto)
        {
            var resourceInDB = await IdentityResourcesService.GetIdentityResourceAsync(dto.Id);

            var updated = _mapper.Map(dto, resourceInDB);

            await IdentityResourcesService.UpdateIdentityResourceAsync(updated);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIdentityResource(Guid id)
        {

            await IdentityResourcesService.DeleteIdentityResourceAsync(id);
            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteIdentityResources([FromBody] List<Guid> ids)
        {

            await IdentityResourcesService.DeleteIdentityResourceAsync(ids.ToArray());
            return NoContent();
        }


    }
}
