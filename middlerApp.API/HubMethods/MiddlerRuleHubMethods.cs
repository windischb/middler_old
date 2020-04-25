using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AutoMapper;
using middler.Common.Storage;
using middler.Hosting.Models;
using middlerApp.API.Helper;
using NamedServices.Microsoft.Extensions.DependencyInjection;
using Reflectensions.ExtensionMethods;
using SignalARRR.Attributes;
using SignalARRR.Server;

namespace middlerApp.API.HubMethods {

    [MessageName("MiddlerRule")]
    public class MiddlerRuleHubMethods: ServerMethods<UIHub> {

        private IMiddlerStorage Repo { get; }

        public MiddlerRuleHubMethods(IServiceProvider serviceProvider) {
            Repo = serviceProvider.GetNamedService<IMiddlerStorage>("litedb");
        }


        public async Task<IEnumerable<MiddlerRuleDto>> GetAll() {
            var rules = await Repo.GetAllAsync();

            return rules.Select(ToDto);
        }

        public async Task<MiddlerRuleDto> Get(Guid id)
        {
            var rule = await Repo.GetByIdAsync(id);

            return ToDto(rule);
        }

        public List<KeyValuePair<string, string>> GetTypings()
        {
            var typings =
                Directory.GetFileSystemEntries(PathHelper.GetFullPath(@"TypeDefinitions"))
                    .Select(fe =>
                    {
                        var f = new FileInfo(fe);
                        return new KeyValuePair<string, string>(f.Name, System.IO.File.ReadAllText(fe));
                    }).ToList();

            return typings;
        }


        public IObservable<MiddlerStorageEventDto> Subscribe() {
            return Repo.EventObservable.Select(ev => new MiddlerStorageEventDto(ev.Action, ToDto(ev.Entity)));
        }


        private MiddlerRuleDto ToDto(MiddlerRuleDbModel dbModel)
        {
            var dto = Converter.CopyTo<MiddlerRuleDto>(dbModel);
            dto.Actions = dto.Actions.Select(action =>
            {
                action.Parameters.Remove("CompiledCode");
                return action;
            }).ToList();
            return dto;
        }

    }
}
