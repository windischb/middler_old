using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;

namespace middlerApp.API.IDP.Services
{
    public class MUserInfoResponseGenerator: UserInfoResponseGenerator
    {
        public MUserInfoResponseGenerator(IProfileService profile, IResourceStore resourceStore, ILogger<UserInfoResponseGenerator> logger) : base(profile, resourceStore, logger)
        {
        }

        public override Task<Dictionary<string, object>> ProcessAsync(UserInfoRequestValidationResult validationResult)
        {
            return base.ProcessAsync(validationResult);
        }

        protected override Task<IEnumerable<string>> GetRequestedClaimTypesAsync(ResourceValidationResult resourceValidationResult)
        {
            IEnumerable<string> result = null;

            if (resourceValidationResult == null)
            {
                result = Enumerable.Empty<string>();
            }
            else
            {
                var identityResources = resourceValidationResult.Resources.IdentityResources;
                result = identityResources.SelectMany(x => x.UserClaims).Distinct();
            }

            return Task.FromResult(result);
        }

        protected override Task<ResourceValidationResult> GetRequestedResourcesAsync(IEnumerable<string> scopes)
        {
            return base.GetRequestedResourcesAsync(scopes);
        }

        
    }
}
