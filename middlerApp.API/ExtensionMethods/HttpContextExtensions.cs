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

            return httpContext.Connection.LocalPort == config.AdminSettings.HttpsPort;

        }

        
    }
}
