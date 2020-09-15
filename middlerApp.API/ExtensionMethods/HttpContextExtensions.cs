using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace middlerApp.API.ExtensionMethods
{
    public static class HttpContextExtensions
    {

        public static bool IsAdminAreaRequest(this HttpContext httpContext)
        {

            var config = httpContext.RequestServices.GetRequiredService<IConfiguration>().Get<StartUpConfiguration>();

            var isAdmin = httpContext.Connection.LocalPort == config.AdminSettings.HttpsPort;
            return isAdmin;

        }

        public static bool IsIdpAreaRequest(this HttpContext httpContext)
        {

            var config = httpContext.RequestServices.GetRequiredService<IConfiguration>().Get<StartUpConfiguration>();

            var isAdmin = httpContext.Connection.LocalPort == config.IdpSettings.HttpsPort;
            return isAdmin;

        }


    }
}
