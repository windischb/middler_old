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
    [MessageName("IDPApiResources")]
    public class IDPApiResourcesServerMethodsHub : ServerMethods<UIHub>
    {
        public IApiResourcesService ApiResourcesService { get; }
        public DataEventDispatcher EventDispatcher { get; }
        private readonly IMapper _mapper;


        public IDPApiResourcesServerMethodsHub(IApiResourcesService apiResourcesService, IMapper mapper, DataEventDispatcher eventDispatcher)
        {
            ApiResourcesService = apiResourcesService;
            EventDispatcher = eventDispatcher;
            _mapper = mapper;

        }


        public async Task<List<MApiResourceListDto>> GetApiResourcesList()
        {
            var resources = await ApiResourcesService.GetAllApiResourceDtosAsync();
            return resources;
        }

        public IObservable<DataEvent> Subscribe()
        {
            return EventDispatcher.Notifications.Where(ev => ev.Subject == "IDPApiResources");
        }
    }
}
