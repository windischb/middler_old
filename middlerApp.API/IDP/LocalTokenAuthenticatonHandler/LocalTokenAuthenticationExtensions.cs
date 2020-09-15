using System;
using Microsoft.AspNetCore.Authentication;

namespace middlerApp.API.IDP.LocalTokenAuthenticatonHandler
{
    public static class LocalTokenAuthenticationExtensions
    {
        public static AuthenticationBuilder AddLocalAccessTokenValidation(this AuthenticationBuilder builder)
            => builder.AddLocalAccessTokenValidation("IdentityServerAccessToken", _ => { });

        public static AuthenticationBuilder AddLocalAccessTokenValidation(this AuthenticationBuilder builder, Action<LocalTokenAuthenticationOptions> configureOptions)
            => builder.AddLocalAccessTokenValidation("IdentityServerAccessToken", configureOptions);

        public static AuthenticationBuilder AddLocalAccessTokenValidation(this AuthenticationBuilder builder, string authenticationScheme, Action<LocalTokenAuthenticationOptions> configureOptions)
            => builder.AddLocalAccessTokenValidation(authenticationScheme, displayName: "Local Authentication", configureOptions: configureOptions);

        public static AuthenticationBuilder AddLocalAccessTokenValidation(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<LocalTokenAuthenticationOptions> configureOptions)
        {
            return builder.AddScheme<LocalTokenAuthenticationOptions, LocalTokenAuthenticationHandler>(authenticationScheme, displayName, configureOptions);
        }
    }
}
