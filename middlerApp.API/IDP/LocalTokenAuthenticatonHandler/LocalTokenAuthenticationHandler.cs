using System;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


namespace middlerApp.API.IDP.LocalTokenAuthenticatonHandler
{
    public class LocalTokenAuthenticationHandler : AuthenticationHandler<LocalTokenAuthenticationOptions>
    {

        private readonly ITokenValidator _tokenValidator;

        //private IAppCache _cache = new CachingService();

        public LocalTokenAuthenticationHandler(IOptionsMonitor<LocalTokenAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, ITokenValidator tokenValidator) : base(options, logger, encoder, clock)
        {
            _tokenValidator = tokenValidator;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            
            string authorization = Request.Headers["Authorization"];

            if (string.IsNullOrEmpty(authorization))
            {
                return AuthenticateResult.Fail("No Authorization Header is sent.");
            }

            return await ValidateTokenAsync(authorization, Scheme.Name, Options.ExpectedScope);
        }

        public async Task<AuthenticateResult> ValidateTokenAsync(string accessToken, string schemeName, string expectedScope, string nameClaim = "name", string roleClaim = "role")
        {

            
             
            if (accessToken.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                accessToken = accessToken.Substring("Bearer ".Length).Trim();
            }

            if (string.IsNullOrEmpty(accessToken))
            {
                return AuthenticateResult.Fail("No Access Token is sent.");
            }


            //var authResult = _cache.Get<AuthenticateResult>(accessToken);

            //if (authResult != null)
            //    return authResult;


            var authResult = await BuildResult(accessToken, schemeName, expectedScope, nameClaim, roleClaim);

            TimeSpan ts = authResult.Succeeded ? TimeSpan.FromMinutes(3) : TimeSpan.FromSeconds(10);

            return authResult; // await _cache.GetOrAddAsync(accessToken, () => Task.FromResult(authResult),ts);


        }

        private async Task<AuthenticateResult> BuildResult(string accessToken, string schemeName, string expectedScope, string nameClaim = "name", string roleClaim = "role")
        {
            TokenValidationResult result =
                await _tokenValidator.ValidateAccessTokenAsync(accessToken, expectedScope);

            if (result.IsError)
            {
                return AuthenticateResult.Fail(result.Error);
            }

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(result.Claims, schemeName, nameClaim, roleClaim);
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            AuthenticationTicket authenticationTicket = new AuthenticationTicket(claimsPrincipal, schemeName);
            return AuthenticateResult.Success(authenticationTicket);

        }
    }
}
