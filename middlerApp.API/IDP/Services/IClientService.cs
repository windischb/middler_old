using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using middlerApp.API.IDP.DtoModels;
using middlerApp.API.IDP.Storage.Entities;

namespace middlerApp.API.IDP.Services
{
    public interface IClientService
    {

        Task<List<MClientDto>> GetAllClientDtosAsync();

        Task<Client> GetClientAsync(Guid id);

        Task<MClientDto> GetClientDtoAsync(Guid id);

        Task CreateClientAsync(MClientDto clientDto);


        Task UpdateClientAsync(Client updated);
        Task DeleteClientAsync(params Guid[] id);
    }
}
