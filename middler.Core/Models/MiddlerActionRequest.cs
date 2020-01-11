using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using middler.Common.Interfaces;
using middler.Core.ExtensionMethods;

namespace middler.Core.Models
{
    public class MiddlerActionRequest : IMiddlerActionRequest
    {
        public string PathTemplate { get; set; }
        public Uri Uri { get; set; }
        public Dictionary<string, object> RouteData { get; set; }
        public string ClientIp { get; set; }
        public string[] ProxyServers { get; set; }

        public MiddlerActionRequest(HttpContext httpContext, MiddlerRuleMatch ruleMatch)
        {
            Uri = new Uri(httpContext.Request.GetDisplayUrl());
            ClientIp = httpContext.Request.FindSourceIp().First().ToString();
            ProxyServers = httpContext.Request.FindSourceIp().Skip(1).Select(ip => ip.ToString()).ToArray();
            PathTemplate = ruleMatch.MiddlerRule.Path;
            RouteData = ruleMatch.RouteData;
        }
    }
}