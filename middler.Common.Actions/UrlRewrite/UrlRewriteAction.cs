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

        public override bool Terminating => false;

        public override bool WriteStreamDirect => false;
        public override string ActionType => DefaultActionType;


        public void ExecuteRequest(IMiddlerContext middlerContext)
        {

            var isAbsolute = Uri.IsWellFormedUriString(Parameters.RewriteTo, UriKind.Absolute);
            if (isAbsolute)
            {
                middlerContext.Request.Uri = new Uri(Parameters.RewriteTo);
            }
            else
            {
                var builder = new UriBuilder(middlerContext.Request.Uri);
                builder.Path = Parameters.RewriteTo;
                middlerContext.Request.Uri = builder.Uri;
            }



            //middlerContext.ForwardBody();

            //var nUrl = new UriBuilder(Parameters.RewriteTo);
            //builder.Path = Parameters.RewriteTo;

        }

        public void ExecuteResponse(IMiddlerContext middlerContext)
        {
            middlerContext.Response.Headers["TestHeader"] = "irgendwas";
           
        }
    }
}
