﻿using System;
using System.Collections.Generic;
using System.Linq;
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
    [Route("api/idp/clients")]
    [AdminController]
    [Authorize(IdentityServerConstants.LocalApi.PolicyName)]
    public class ClientsController: Controller
    {
        public IClientService ClientService { get; }
        private readonly IMapper _mapper;


        public ClientsController(IClientService clientService, IMapper mapper)
        {
            ClientService = clientService;
            _mapper = mapper;

        }


        [HttpGet]
        public async Task<ActionResult<List<MClientDto>>> GetAllClients()
        {
            var clients = await ClientService.GetAllClientDtosAsync();
            return Ok(clients);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MUserDto>> GetClient(string id)
        {

            if (id == "create")
            {
                return Ok(new MClientDto());
            }

            if (!Guid.TryParse(id, out var guid))
                return NotFound();

            var client = await ClientService.GetClientDtoAsync(guid);

            if (client == null)
                return NotFound();

            return Ok(client);
            
        }

        [HttpPost]
        public async Task<IActionResult> CreateClient(MClientDto createClientDto)
        {

            await ClientService.CreateClientAsync(createClientDto);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateClient(MClientDto roleDto)
        {
            var roleInDB = await ClientService.GetClientAsync(roleDto.Id);

            var secrets = roleInDB.ClientSecrets.ToDictionary(sec => sec.Id, sec => sec);
            var newSecrets = new List<ClientSecret>();
            foreach (var dtoSecret in roleDto.ClientSecrets)
            {
                ClientSecret sec = null;
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
                    sec = _mapper.Map<ClientSecret>(dtoSecret);

                }

                if (sec != null)
                {
                    newSecrets.Add(sec);
                }
            }

            var updated = _mapper.Map(roleDto, roleInDB);
            updated.ClientSecrets = newSecrets;

            await ClientService.UpdateClientAsync(updated);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(Guid id)
        {

            await ClientService.DeleteClientAsync(id);
            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteRoles([FromBody] List<Guid> ids)
        {

            await ClientService.DeleteClientAsync(ids.ToArray());
            return NoContent();
        }
    }
}
