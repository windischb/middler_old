using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using middler.Common.Models;

namespace middler.Common.Interfaces
{
    public interface IMiddlerMap
    {
        List<MiddlerRule> GetFlatList(IServiceProvider serviceProvider);
        void AddRule(params MiddlerRule[] middlerRules);
        void AddRepo<T>();
        void AddNamedRepo(string name);
    }
}
