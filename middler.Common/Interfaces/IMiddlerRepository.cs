using System.Collections.Generic;
using middler.Common.Models;

namespace middler.Common.Interfaces
{
    public interface IMiddlerRepository {
        
        List<MiddlerRule> ProvideRules();
    }
   
}
