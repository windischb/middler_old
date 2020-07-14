using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Services;
using Microsoft.EntityFrameworkCore;
using middler.IDP.DbContexts;

namespace middler.IDP.Services
{
    public class MCorsPolicyService: ICorsPolicyService
    {
        public IdentityDbContext DbContext { get; }

        public MCorsPolicyService(IdentityDbContext dbContext)
        {
            DbContext = dbContext;
        }
        public Task<bool> IsOriginAllowedAsync(string origin)
        {
            return DbContext.Clients.AnyAsync(c => c.AllowedCorsOrigins.Select(o => o.Origin).Contains(origin));
        }
    }
}
