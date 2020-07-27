using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AutoMapper;
using middlerApp.API.IDP.DtoModels;
using middlerApp.API.IDP.Models;
using middlerApp.API.IDP.Services;
using SignalARRR.Attributes;
using SignalARRR.Server;

namespace middlerApp.API.HubMethods
{
    [MessageName("IDPClients")]
    public class IDPClientsServerMethodsHub : ServerMethods<UIHub>
    {
        public IClientService ClientService { get; }
        public DataEventDispatcher EventDispatcher { get; }
        private readonly IMapper _mapper;


        public IDPClientsServerMethodsHub(IClientService clientService, IMapper mapper, DataEventDispatcher eventDispatcher)
        {
            ClientService = clientService;
            EventDispatcher = eventDispatcher;
            _mapper = mapper;

        }


        public async Task<List<MClientDto>> GetClientsList()
        {
            var roles = await ClientService.GetAllClientDtosAsync();
            return roles;
        }

        public IObservable<DataEvent> Subscribe()
        {
            return EventDispatcher.Notifications.Where(ev => ev.Subject == "IDPClients");
        }
    }
}
