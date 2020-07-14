using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Services;
using Microsoft.EntityFrameworkCore;

namespace middlerApp.API.IDP.Services
{
    public class MCorsPolicyService: ICorsPolicyService
    {
        public IDPDbContext DbContext { get; }

        public MCorsPolicyService(IDPDbContext dbContext)
        {
            DbContext = dbContext;
        }
        public Task<bool> IsOriginAllowedAsync(string origin)
        {
            return DbContext.Clients.AnyAsync(c => c.AllowedCorsOrigins.Select(o => o.Origin).Contains(origin));
        }
    }
}
