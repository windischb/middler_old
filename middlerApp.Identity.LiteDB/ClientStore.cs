using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using LiteDB;
using middlerApp.Identity.Models.Models;

namespace middlerApp.Identity.LiteDB
{
    public class ClientStore : IClientStore
    {
        private readonly ConfigurationDbContext _context;
        public ClientStore(ConfigurationDbContext context)
        {

            _context = context;

        }

        public Task<Client> FindClientByIdAsync(string clientId)
        {
            var client = _context.Clients.FirstOrDefault(x => x.ClientId == clientId);

            var model = client;


            return Task.FromResult(model);
        }
    }
}
