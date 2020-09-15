using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Hosting.LocalApiAuthentication;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authentication;
using middlerApp.API.IDP.LocalTokenAuthenticatonHandler;
using SignalARRR.Server;

namespace middlerApp.API
{
    //public class OAuthAuthenticator: IAuthenticator
    //{

    //    private readonly ITokenValidator _tokenValidator;


    //    public OAuthAuthenticator(ITokenValidator tokenValidator)
    //    {

    //        _tokenValidator = tokenValidator;
    //    }

    //    public async Task<(bool authenticated, ClaimsPrincipal principal)> TryAuthenticate(string authData)
    //    {

    //        if (String.IsNullOrWhiteSpace(authData))
    //        {
    //            return (false, new ClaimsPrincipal());
    //        }
    //        TokenValidationResult result =
    //            await _tokenValidator.ValidateAccessTokenAsync(authData, null);

    //        if (result.IsError)
    //        {
    //            return (false, new ClaimsPrincipal());
    //        }

    //        ClaimsIdentity claimsIdentity = new ClaimsIdentity(result.Claims, "IdentityServerAccessToken", "name", "role");
    //        ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
    //        //AuthenticationTicket authenticationTicket = new AuthenticationTicket(claimsPrincipal, "IdentityServerAccessToken");

    //        return (true, claimsPrincipal);

    //    }
    //}
}
