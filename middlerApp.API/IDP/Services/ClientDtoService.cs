using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using middlerApp.API.IDP.DtoModels;
using middlerApp.API.IDP.Storage.Entities;

namespace middlerApp.API.IDP.Services
{
    public class ClientDtoService: IClientDtoService
    {
        private readonly IMapper _mapper;
        private IDPDbContext DbContext { get; }

        public ClientDtoService(IDPDbContext dbContext, IMapper mapper)
        {
            _mapper = mapper;
            DbContext = dbContext;
        }
        public async Task<List<MClientDto>> GetAllClientDtosAsync()
        {
            var clients = await DbContext.Clients.ToListAsync();
            return _mapper.Map<List<MClientDto>>(clients);
        }

        public async Task<MClientDto> GetClientDtoAsync(Guid id)
        {
            var client = await DbContext.Clients.FirstOrDefaultAsync(c => c.Id == id);
            return _mapper.Map<MClientDto>(client);
        }

        public async Task CreateClientAsync(MClientDto clientDto)
        {
            var client = _mapper.Map<Client>(clientDto);
            await DbContext.Clients.AddAsync(client);
            await DbContext.SaveChangesAsync();
        }
    }
}
