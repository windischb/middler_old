using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Constraints;
using middler.Common;
using middler.Common.SharedModels.Models;
using middler.Common.StreamHelper;
using middler.Core.ExtensionMethods;
using Reflectensions.ExtensionMethods;

namespace middler.Core
{
    public class MiddlerRequestContext:  IMiddlerRequestContext
    {
        //public PathString Path { get; set; }
        public ClaimsPrincipal Principal { get; set; } 
        public IPAddress SourceIPAddress { get; set; }
        public List<IPAddress> ProxyServers { get; set; }
        public MiddlerRouteData RouteData { get; set; } = new MiddlerRouteData();
        public SimpleDictionary<string> Headers { get; set; }
        public MiddlerRouteQueryParameters QueryParameters { get; set; }

        public Uri Uri { get; set; }


        public string HttpMethod { get; set; }

        public CancellationToken RequestAborted => HttpContext.RequestAborted;
        public string ContentType { get; set; }
       
        public Stream Body { get; private set; }

        private HttpContext HttpContext { get; }



        public MiddlerRequestContext(HttpContext httpContext)
        {
            HttpContext = httpContext;
            //Path = httpContext.Request.Path;
            Principal = httpContext.User;
            SourceIPAddress = httpContext.Request.FindSourceIp().FirstOrDefault();
            ProxyServers = httpContext.Request.FindSourceIp().Skip(1).ToList();
            Headers = new SimpleDictionary<string>(httpContext.Request.GetHeaders());
            QueryParameters = new MiddlerRouteQueryParameters(httpContext.Request.GetQueryParameters());
            Uri = new Uri(httpContext.Request.GetDisplayUrl());
            HttpMethod = httpContext.Request.Method;
            ContentType = httpContext.Request.ContentType;

            Body = httpContext.Request.Body;
        }

        public RouteData GetRouteData()
        {
            return HttpContext.GetRouteData();
        }

        internal void SetRouteData(IDictionary<string, IRouteConstraint> constraints)
        {
            var middlerRouteData = new MiddlerRouteData();

            middlerRouteData["@HOST"] = Uri.Host;
            middlerRouteData["@SCHEME"] = Uri.Scheme;
            middlerRouteData["@PORT"] = Uri.Port;
            middlerRouteData["@PATH"] = Uri.AbsolutePath;

            var rd = GetRouteData();

            foreach (var key in rd.Values.Keys)
            {
                var val = rd.Values[key]?.ToString();

                if (val == null)
                {
                    middlerRouteData.Add(key.ToLower(), null);

                    continue;
                }

                if (constraints.ContainsKey(key))
                {

                    var constraint = constraints[key];
                    IRouteConstraint ic;
                    if (constraint is OptionalRouteConstraint optionalRouteConstraint)
                    {
                        ic = optionalRouteConstraint.InnerConstraint;
                    }
                    else
                    {
                        ic = constraint;
                    }

                    object value;
                    if (ic is IntRouteConstraint)
                    {
                        value = val.ToInt();
                    }
                    else if (ic is BoolRouteConstraint)
                    {
                        value = val.ToBoolean();
                    }
                    else if (ic is DateTimeRouteConstraint)
                    {
                        value = val.ToDateTime();
                    }
                    else if (ic is DecimalRouteConstraint)
                    {
                        value = val.ToDecimal();
                    }
                    else if (ic is DoubleRouteConstraint)
                    {
                        value = val.ToDouble();
                    }
                    else if (ic is FloatRouteConstraint)
                    {
                        value = val.ToFloat();
                    }
                    else if (ic is GuidRouteConstraint)
                    {
                        value = new Guid(val);
                    }
                    else if (ic is LongRouteConstraint)
                    {
                        value = val.ToLong();
                    }
                    else
                    {
                        value = val;
                    }

                    middlerRouteData.Add(key.ToLower(), value);
                }
                else
                {
                    middlerRouteData.Add(key.ToLower(), val);
                }

            }

        }

        internal void SetNextBody(Stream stream)
        {
            this.Body = stream;
        }

        public string GetBodyAsString()
        {
            using var sr = new StreamReader(Body);
            return sr.ReadToEndAsync().GetAwaiter().GetResult();
        }
    }
}
