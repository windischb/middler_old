using System;
using Microsoft.AspNetCore.Http;
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


        public void ExecuteRequest(IMiddlerActionContext actionContext)
        {

            var builder = new UriBuilder(actionContext.Request.Uri);
            builder.Path = Parameters.RewriteTo;
            actionContext.HttpContext.Request.Path = builder.Uri.LocalPath;
        }

        public void ExecuteResponse(IMiddlerActionContext actionContext)
        {
            actionContext.HttpContext.Response.Headers.Add("TestHeader", "irgendwas");
            actionContext.HttpContext.Response.WriteAsync($" - {GetType().Name}");
        }
    }
}
