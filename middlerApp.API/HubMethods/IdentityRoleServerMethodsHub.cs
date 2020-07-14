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
    [MessageName("IdentityRoles")]
    public class IdentityRoleServerMethodsHub: ServerMethods<UIHub>
    {
        public IRolesService RolesService { get; }
        public DataEventDispatcher EventDispatcher { get; }
        private readonly IMapper _mapper;


        public IdentityRoleServerMethodsHub(IRolesService rolesService, IMapper mapper, DataEventDispatcher eventDispatcher)
        {
            RolesService = rolesService;
            EventDispatcher = eventDispatcher;
            _mapper = mapper;

        }


        public async Task<List<MRoleListDto>> GetRolesList()
        {
            var roles = await RolesService.GetAllRoleListDtosAsync();
            return roles;
        }

        public IObservable<DataEvent> Subscribe()
        {
            return EventDispatcher.Notifications.Where(ev => ev.Subject == "IdentityRole");
        }
    }
}
