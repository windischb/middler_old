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
    public class ClientService: IClientService
    {
        private readonly IMapper _mapper;
        private IDPDbContext DbContext { get; }

        public DataEventDispatcher EventDispatcher { get; }

        public ClientService(IDPDbContext dbContext, IMapper mapper, DataEventDispatcher eventDispatcher)
        {
            _mapper = mapper;
            DbContext = dbContext;
            EventDispatcher = eventDispatcher;
        }
        public async Task<List<MClientDto>> GetAllClientDtosAsync()
        {
            var clients = await DbContext.Clients.ToListAsync();
            return _mapper.Map<List<MClientDto>>(clients);
        }

        public async Task<Client> GetClientAsync(Guid id)
        {
            return await DbContext.Clients.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<MClientDto> GetClientDtoAsync(Guid id)
        {
            var client = await GetClientAsync(id);
            return _mapper.Map<MClientDto>(client);
        }

        public async Task CreateClientAsync(MClientDto clientDto)
        {
            var client = _mapper.Map<Client>(clientDto);
            await DbContext.Clients.AddAsync(client);
            await DbContext.SaveChangesAsync();

            EventDispatcher.DispatchCreatedEvent("IdentityClients", _mapper.Map<MClientDto>(client));
        }

        public async Task DeleteClientAsync(params Guid[] id)
        {
            var clients = await DbContext.Clients.Where(u => id.Contains(u.Id)).ToListAsync();
            DbContext.Clients.RemoveRange(clients);
            await DbContext.SaveChangesAsync();

            EventDispatcher.DispatchDeletedEvent("IdentityClients", clients.Select(r => r.Id));
        }

        public async Task UpdateClientAsync(Client updated)
        {
            //var roleModel = _mapper.Map<MRole>(updated);
            //var userIds = updated.UserRoles.Select(ur => ur.UserId).ToList();
            //var availableUsers = DbContext.Users.Where(r => userIds.Contains(r.Id)).Select(r => r.Id).ToList();

            //updated.UserRoles = updated.UserRoles.Where(ur => availableUsers.Contains(ur.UserId)).ToList();

            await DbContext.SaveChangesAsync();

            EventDispatcher.DispatchUpdatedEvent("IdentityClients", _mapper.Map<MClientDto>(updated));
        }
    }
}
