using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using middler.Action.Scripting.ExtensionMethods;
using middler.Action.Scripting.Shared;
using middler.Common.SharedModels.Models;

namespace middler.Action.Scripting.Models
{
    public class ScriptContextRequest: IScriptContextRequest
    {
        public string HttpMethod { get; }
        public Uri Uri { get; }
        public Dictionary<string, object> RouteData { get; set; }
        public Dictionary<string, object> Headers { get; set; }
        public Dictionary<string, string> QueryParameters { get; set; }
        public string UserAgent { get; }
        public string ClientIp { get; }
        public string[] ProxyServers { get; }

        public ScriptContextRequest(HttpContext httpContext)
        {
            HttpMethod = httpContext.Request.Method;
            Uri = new Uri(httpContext.Request.GetDisplayUrl());
            UserAgent = httpContext.Request.Headers["User-Agent"].ToString();
            ClientIp = httpContext.Request.FindSourceIp().FirstOrDefault()?.ToString();
            ProxyServers = httpContext.Request.FindSourceIp().Skip(1).Select(ip => ip.ToString()).ToArray();
            RouteData = new RuleRouteData(httpContext.Features.Get<MiddlerRouteData>());
            Headers = new SimpleDictionary<object>(httpContext.Request.GetHeaders());
            QueryParameters = new ScriptContextQueryParameters(httpContext.Request.GetQueryParameters());
        }
    }
}