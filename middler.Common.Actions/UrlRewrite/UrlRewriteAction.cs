using System;
using System.Text;
using Microsoft.AspNetCore.Http;
using middler.Common.ExtensionMethods;
using middler.Common.Interfaces;
using middler.Common.Models;

namespace middler.Common.Actions.UrlRewrite
{
    public class UrlRewriteAction: MiddlerAction<UrlRewriteOptions>
    {
        
        public override bool ContinueAfterwards => true;

        public override bool WriteStreamDirect => false;


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
