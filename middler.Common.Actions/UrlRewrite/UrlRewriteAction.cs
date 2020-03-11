using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using middler.Common.Interfaces;
using middler.Common.SharedModels.Models;

namespace middler.Common.Actions.UrlRewrite
{
    public class UrlRewriteAction: MiddlerAction<UrlRewriteOptions>
    {
        internal static string DefaultActionType => "UrlRewrite";

        public override bool ContinueAfterwards => true;

        public override bool WriteStreamDirect => false;
        public override string ActionType => DefaultActionType;


        public void ExecuteRequest(HttpContext httpContext)
        {

            var builder = new UriBuilder(httpContext.Request.GetDisplayUrl());
            builder.Path = Parameters.RewriteTo;
            httpContext.Request.Path = builder.Uri.LocalPath;
        }

        public void ExecuteResponse(HttpContext httpContext)
        {
            httpContext.Response.Headers["TestHeader"] = "irgendwas";
            httpContext.Response.WriteAsync($" - {GetType().Name}");
        }
    }
}
