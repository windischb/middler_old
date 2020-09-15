using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Validation;

namespace middlerApp.API.IDP.Validators
{
    public class MCustomTokenRequestValidator : ICustomTokenRequestValidator
    {
        //private XAClientStore ClientStore { get; }
        //private XARolesManager RolesManager { get; }

        //private XAApiResourceManager ApiResourceManager { get; }

        //public MCustomTokenRequestValidator(XAClientStore clientStore, XARolesManager rolesManager, XAApiResourceManager apiResourceManager)
        //{
        //    ClientStore = clientStore;
        //    RolesManager = rolesManager;
        //    ApiResourceManager = apiResourceManager;
        //}

        public async Task ValidateAsync(CustomTokenRequestValidationContext context)
        {

            var client = context.Result.ValidatedRequest.Client;
           
            //if (context.Result.ValidatedRequest.GrantType == "client_credentials")
            //{

            //    var resources = context.Result.ValidatedRequest.ValidatedScopes.GrantedResources.ApiResources;

            //    var apiResources = await ApiResourceManager.GetApiResourcesByNameAsync(resources.Select(r => r.Name));

            //    var cl = await ClientStore.Storage.QueryAsync(where => where.Eq(c => c.ClientId, client.ClientId)).FirstOrDefaultAsync();

            //    var roles = await RolesManager.GetRolesFromClientRecursive(cl, apiResources);


            //    var tempClaims = context.Result.ValidatedRequest
            //        .ClientClaims
            //        .Where(c => c.Type.ToLower() != "role")
            //        .Where(c => c.Type.ToLower() != "name")
            //        .ToList();

            //    tempClaims.AddRange(roles.Values.Select(r => new Claim("role", r.GenerateRoleName(cl.RoleAffix, apiResources))));
            //    context.Result.ValidatedRequest.ClientClaims = tempClaims;

            //}

            return;

        }



    }
}