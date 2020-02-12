using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AutoMapper;
using middler.Common.Storage;
using middler.Hosting.Models;
using NamedServices.Microsoft.Extensions.DependencyInjection;
using Reflectensions.ExtensionMethods;
using SignalARRR.Attributes;
using SignalARRR.Server;

namespace middlerApp.API.HubMethods {

    [MessageName("MiddlerRule")]
    public class MiddlerRuleHubMethods: ServerMethods<UIHub> {

        private readonly IMapper _mapper;
        private IMiddlerStorage Repo { get; }

        public MiddlerRuleHubMethods(IServiceProvider serviceProvider, IMapper mapper) {
            _mapper = mapper;
            Repo = serviceProvider.GetNamedService<IMiddlerStorage>("litedb");
        }


        public async Task<IEnumerable<MiddlerRuleDto>> GetAll() {
            var rules = await Repo.GetAllAsync();
            
            return _mapper.Map<IEnumerable<MiddlerRuleDto>>(rules);
        }

        public async Task<MiddlerRuleDto> Get(Guid id)
        {
            var rule = await Repo.GetByIdAsync(id);

            return _mapper.Map<MiddlerRuleDto>(rule);
        }


        public IObservable<MiddlerStorageEventDto> Subscribe() {
            return Repo.EventObservable.Select(ev => new MiddlerStorageEventDto(ev.Action, _mapper.Map<MiddlerRuleDto>(ev.Entity)));
        }

    }
}
