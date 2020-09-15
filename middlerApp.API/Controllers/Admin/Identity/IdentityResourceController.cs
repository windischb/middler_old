using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using IdentityServer4;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using middlerApp.API.Attributes;
using middlerApp.API.IDP.DtoModels;
using middlerApp.API.IDP.Services;
using middlerApp.API.IDP.Storage.Entities;

namespace middlerApp.API.Controllers.Admin.Identity
{
    [ApiController]
    [Route("api/idp/identity-resources")]
    [AdminController]
    [Authorize(IdentityServerConstants.LocalApi.PolicyName)]
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
        public async Task<ActionResult<List<MScopeListDto>>> GetList()
        {

            var resources = await IdentityResourcesService.GetAllIdentityResourceDtosAsync();
            return Ok(resources);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MScopeDto>> Get(string id)
        {

            if (id == "create")
            {
                return Ok(_mapper.Map<MScopeDto>(new Scope()));
            }

            if (!Guid.TryParse(id, out var guid))
                return NotFound();

            var apiResource = await IdentityResourcesService.GetIdentityResourceDtoAsync(guid);

            if (apiResource == null)
                return NotFound();

          
            return Ok(apiResource);


        }

        [HttpPost]
        public async Task<IActionResult> CreateIdentityResource(MScopeDto dto)
        {
            await IdentityResourcesService.CreateIdentityResourceAsync(dto);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateIdentityResource(MScopeDto dto)
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
