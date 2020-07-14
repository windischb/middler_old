using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using middlerApp.API.IDP.DtoModels;

namespace middlerApp.API.IDP.Services
{
    public interface IClientDtoService
    {

        Task<List<MClientDto>> GetAllClientDtosAsync();

        Task<MClientDto> GetClientDtoAsync(Guid id);

        Task CreateClientAsync(MClientDto clientDto);

    }
}
