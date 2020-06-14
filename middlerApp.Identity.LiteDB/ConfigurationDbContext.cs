using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityServer4.Models;
using LiteDB;
using middlerApp.Identity.Models.Models;

namespace middlerApp.Identity.LiteDB
{
    public class ConfigurationDbContext : IdentityDbContext
    {
        private ILiteCollection<MClient> _clients;
        private ILiteCollection<IdentityResource> _identityResources;
        private ILiteCollection<ApiResource> _apiResources;

        public ConfigurationDbContext(string settings)
            : base(settings)
        {
            _clients = Database.GetCollection<MClient>("Clients");
            _identityResources = Database.GetCollection<IdentityResource>("IdentityResources");
            _apiResources = Database.GetCollection<ApiResource>("ApiResources");
        }

        public IQueryable<Client> Clients
        {
            get { return _clients.FindAll().AsQueryable(); }
        }

        public IQueryable<IdentityResource> IdentityResources
        {
            get { return _identityResources.FindAll().AsQueryable(); }
        }

        public IQueryable<ApiResource> ApiResources
        {
            get { return _apiResources.FindAll().AsQueryable(); }
        }

        public async Task AddClient(MClient entity)
        {
            _clients.Insert(entity);
        }

        public async Task AddIdentityResource(IdentityResource entity)
        {
            _identityResources.Insert(entity);
        }

        public async Task AddApiResource(ApiResource entity)
        {
            _apiResources.Insert(entity);
        }

    }
}
