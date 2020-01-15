using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using middler.Core.IPHelper;

namespace middlerApp.Server.ExtensionMethods
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
