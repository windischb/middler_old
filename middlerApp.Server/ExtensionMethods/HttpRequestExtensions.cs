using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Http;

namespace middlerApp.Server.ExtensionMethods
{
    public static class HttpRequestExtensions
    {

        public static List<IPAddress> FindSourceIp(this HttpRequest httpRequest)
        {


            var sourceIps = new List<IPAddress>();


            if (httpRequest.Headers.ContainsKey("X-Forwarded-For"))
            {
                var ips = httpRequest.Headers["X-Forwarded-For"];
                sourceIps = ips.Select(IPAddress.Parse).ToList();
            }

            sourceIps.Add(httpRequest.HttpContext.Connection.RemoteIpAddress);


            return sourceIps;
        }

    }
}


