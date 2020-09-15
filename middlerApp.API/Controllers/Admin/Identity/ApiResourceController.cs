using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using IdentityModel;
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
    [Route("api/idp/api-resources")]
    [AdminController]
    [Authorize(IdentityServerConstants.LocalApi.PolicyName)]
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

            var apiResource = await ApiResourcesService.GetApiResourceDtoAsync(guid);

            if (apiResource == null)
                return NotFound();

          
            return Ok(apiResource);


        }

        [HttpPost]
        public async Task<IActionResult> CreateApiResource(MApiResourceDto dto)
        {
            await ApiResourcesService.CreateApiResourceAsync(dto);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateApiResource(MApiResourceDto dto)
        {
            var resourceInDB = await ApiResourcesService.GetApiResourceAsync(dto.Id);

            var secrets = resourceInDB.Secrets.ToDictionary(sec => sec.Id, sec => sec);
            var newSecrets = new List<ApiResourceSecret>();
            foreach (var dtoSecret in dto.Secrets)
            {
                ApiResourceSecret sec = null;
                if (dtoSecret.Id != Guid.Empty)
                {
                    sec = secrets[dtoSecret.Id];
                    if (sec.Description != dtoSecret.Description)
                    {
                        sec.Description = dtoSecret.Description;
                    }

                    if (sec.Expiration != dtoSecret.Expiration)
                    {
                        sec.Expiration = dtoSecret.Expiration;
                    }
                    
                }
                else
                {
                    dtoSecret.Value = dtoSecret.Value.ToSha512();
                    sec =_mapper.Map<ApiResourceSecret>(dtoSecret);
                    
                }

                if (sec != null)
                {
                    newSecrets.Add(sec);
                }
            }
            
            var updated = _mapper.Map(dto, resourceInDB);
            updated.Secrets = newSecrets;

            await ApiResourcesService.UpdateApiResourceAsync(updated);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteApiResource(Guid id)
        {

            await ApiResourcesService.DeleteApiResourceAsync(id);
            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteApiResources([FromBody] List<Guid> ids)
        {

            await ApiResourcesService.DeleteApiResourceAsync(ids.ToArray());
            return NoContent();
        }


    }
}
