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
    [MessageName("IDPIdentityResources")]
    public class IDPIdentityResourcesServerMethodsHub: ServerMethods<UIHub>
    {
        public IIdentityResourcesService IdentityResourcesService { get; }
        public DataEventDispatcher EventDispatcher { get; }
        private readonly IMapper _mapper;


        public IDPIdentityResourcesServerMethodsHub(IIdentityResourcesService apiResourcesService, IMapper mapper, DataEventDispatcher eventDispatcher)
        {
            IdentityResourcesService = apiResourcesService;
            EventDispatcher = eventDispatcher;
            _mapper = mapper;

        }


        public async Task<List<MScopeListDto>> GetIdentityResourcesList()
        {
            var resources = await IdentityResourcesService.GetAllIdentityResourceDtosAsync();
            return resources;
        }

        public IObservable<DataEvent> Subscribe()
        {
            return EventDispatcher.Notifications.Where(ev => ev.Subject == "IDPIdentityResources");
        }
    }
}
