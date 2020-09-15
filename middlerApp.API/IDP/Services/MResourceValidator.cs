using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;

namespace middlerApp.API.IDP.Services
{
    public class MResourceValidator: DefaultResourceValidator
    {
        public MResourceValidator(IResourceStore store, IScopeParser scopeParser, ILogger<DefaultResourceValidator> logger) : base(store, scopeParser, logger)
        {

        }

        protected override Task<bool> IsClientAllowedIdentityResourceAsync(Client client, IdentityResource identity)
        {
            return base.IsClientAllowedIdentityResourceAsync(client, identity);
        }

        protected override Task<bool> IsClientAllowedApiScopeAsync(Client client, ApiScope apiScope)
        {
            return base.IsClientAllowedApiScopeAsync(client, apiScope);
        }
    }
}
