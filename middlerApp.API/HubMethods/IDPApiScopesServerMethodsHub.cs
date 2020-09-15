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
    [MessageName("IDPApiScopes")]
    public class IDPApiScopesServerMethodsHub : ServerMethods<UIHub>
    {
        public IApiScopesService ApiScopesService { get; }
        public DataEventDispatcher EventDispatcher { get; }
        private readonly IMapper _mapper;


        public IDPApiScopesServerMethodsHub(IApiScopesService apiScopesService, IMapper mapper, DataEventDispatcher eventDispatcher)
        {
            ApiScopesService = apiScopesService;
            EventDispatcher = eventDispatcher;
            _mapper = mapper;

        }

        public IObservable<DataEvent> Subscribe()
        {
            return EventDispatcher.Notifications.Where(ev => ev.Subject == "IDPApiScopes");
        }
    }
}
