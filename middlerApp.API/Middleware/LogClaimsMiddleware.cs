using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using middlerApp.API.Helper;

namespace middlerApp.API.Middleware
{
    public class LogClaimsMiddleware
    {
        private readonly RequestDelegate _next;

        public LogClaimsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        // IMyScopedService is injected into Invoke
        public async Task Invoke(HttpContext httpContext)
        {

            var claims = httpContext?.User?.Claims;
            if (claims == null)
            {
                Console.WriteLine("NO CLAIMS");
            }
            else
            {

                var list = claims.Select(c => new {Type = c.Type, Value = c.Value}).Where(c => c.Type == "role");
                foreach (var x1 in list)
                {
                    Console.WriteLine(x1.Value);
                }
                //Console.WriteLine(Converter.Json.ToJson(list, true));
            }

            await _next(httpContext);
        }
    }
}
