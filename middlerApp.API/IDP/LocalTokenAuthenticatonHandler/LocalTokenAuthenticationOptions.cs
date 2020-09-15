using Microsoft.AspNetCore.Authentication;

namespace middlerApp.API.IDP.LocalTokenAuthenticatonHandler
{
    public class LocalTokenAuthenticationOptions : AuthenticationSchemeOptions
    {
        public string ExpectedScope { get; set; }

        public string NameClaim { get; set; } = "name";
        public string RoleClaim { get; set; } = "role";

    }
}