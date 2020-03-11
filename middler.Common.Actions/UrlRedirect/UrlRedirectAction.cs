using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using middler.Common.Interfaces;
using middler.Common.SharedModels.Models;

namespace middler.Common.Actions.UrlRedirect
{
    public class UrlRedirectAction: MiddlerAction<UrlRedirectOptions>
    {
        internal static string DefaultActionType => "UrlRedirect";
        public void ExecuteRequest(HttpContext httpContext, IActionHelper actionHelper)
        {
            var uri = new Uri(actionHelper.BuildPathFromRoutData(Parameters.RedirectTo));
            httpContext.Response.Redirect(uri.AbsoluteUri, Parameters.Permanent, Parameters.PreserveMethod);
        }

        public override string ActionType => DefaultActionType;
    }
}
