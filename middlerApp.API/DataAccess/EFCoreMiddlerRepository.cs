using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using middler.Common.Interfaces;
using middler.Common.SharedModels.Models;

namespace middlerApp.API.DataAccess
{
    public class EFCoreMiddlerRepository : IMiddlerRepository
    {
        private readonly IMapper _mapper;
        public AppDbContext AppDbContext { get; }

        public EFCoreMiddlerRepository(AppDbContext appDbContext, IMapper mapper)
        {
            _mapper = mapper;
            AppDbContext = appDbContext;
        }

        public List<MiddlerRule> ProvideRules()
        {
            var rules = AppDbContext.EndpointRules.Include(r => r.Actions).ToList()
                .Select(
                    r =>
                    {
                        r.Actions = r.Actions.OrderBy(a => a.Order).ToList();
                        return r;
                    })
                .Select(r => _mapper.Map<MiddlerRule>(r));

            return rules.ToList();

        }
    }
}
