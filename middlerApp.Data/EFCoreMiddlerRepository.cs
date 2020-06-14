using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using middler.Common.Interfaces;
using middler.Common.SharedModels.Models;
using middlerApp.Data.MapperProfiles;

namespace middlerApp.Data
{
    public class EFCoreMiddlerRepository : IMiddlerRepository
    {
        public MiddlerDbContext MiddlerDbContext { get; }

        public EFCoreMiddlerRepository(MiddlerDbContext middlerDbContext)
        {
            MiddlerDbContext = middlerDbContext;
        }

        public List<MiddlerRule> ProvideRules()
        {
            var rules = MiddlerDbContext.EndpointRules.Include(r => r.Actions).ToList()
                .Select(
                    r =>
                    {
                        r.Actions = r.Actions.OrderBy(a => a.Order).ToList();
                        return r;
                    })
                .Select(r => ObjectMapper.Mapper.Map<MiddlerRule>(r));

            return rules.ToList();

        }
    }
}
