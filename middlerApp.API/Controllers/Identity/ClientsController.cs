using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using middlerApp.API.Attributes;
using middlerApp.API.IDP;
using middlerApp.API.IDP.DtoModels;
using middlerApp.API.IDP.Services;
using middlerApp.API.IDP.Storage.Entities;

namespace middlerApp.API.Controllers.Identity
{
    [ApiController]
    [Route("api/identity/clients")]
    [AdminController]
    public class ClientsController: Controller
    {
        public IClientDtoService ClientDtoService { get; }
        private readonly IMapper _mapper;


        public ClientsController(IClientDtoService clientDtoService, IMapper mapper)
        {
            ClientDtoService = clientDtoService;
            _mapper = mapper;

        }


        [HttpGet]
        public async Task<ActionResult<List<MClientDto>>> GetAllClients()
        {
            var clients = await ClientDtoService.GetAllClientDtosAsync();
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

            var client = await ClientDtoService.GetClientDtoAsync(guid);

            if (client == null)
                return NotFound();

            return Ok(client);
            
        }

        [HttpPost]
        public async Task<IActionResult> CreateClient(MClientDto createClientDto)
        {

            await ClientDtoService.CreateClientAsync(createClientDto);
            return Ok();
        }
    }
}
