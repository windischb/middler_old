using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using middlerApp.API.DataAccess;
using middlerApp.API.Helper;
using middlerApp.API.IDP.Services;
using SignalARRR.Attributes;
using SignalARRR.Server;

namespace middlerApp.API.HubMethods {

    [MessageName("MiddlerRule")]
    public class MiddlerRuleHubMethods: ServerMethods<UIHub> {
        
        private readonly EndpointRuleRepository _endpointRuleRepository;

        public DataEventDispatcher EventDispatcher { get; }

        public MiddlerRuleHubMethods(IServiceProvider serviceProvider, EndpointRuleRepository endpointRuleRepository, DataEventDispatcher eventDispatcher)
        {
            _endpointRuleRepository = endpointRuleRepository;
            EventDispatcher = eventDispatcher;
        }


        public async Task<IEnumerable<EndpointRuleEntity>> GetAll() {
            var rules = await _endpointRuleRepository.GetAllAsync();

            return rules;
        }

        public async Task<EndpointRuleEntity> Get(Guid id)
        {
            var rule = await _endpointRuleRepository.GetByIdAsync(id);

            return rule;
        }

        public List<KeyValuePair<string, string>> GetTypings()
        {
            var typings =
                Directory.GetFileSystemEntries(PathHelper.GetFullPath(@"TypeDefinitions"))
                    .Select(fe =>
                    {
                        var f = new FileInfo(fe);
                        return new KeyValuePair<string, string>(f.Name, File.ReadAllText(fe));
                    }).ToList();

            return typings;
        }


        public IObservable<object> Subscribe() {

            return EventDispatcher.Notifications.Where(ev => ev.Subject == "EndpointRule");

        }

    }
}
