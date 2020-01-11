using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using middler.Common.Interfaces;
using middler.Common.Models;

namespace middler.Common.Actions.UrlRedirect
{
    public class UrlRedirectAction: MiddlerAction<UrlRedirectOptions>
    {

        public void ExecuteRequest(IMiddlerActionContext actionContext)
        {
            var uri = new Uri(actionContext.Helper.BuildPathFromRoutData(Parameters.RedirectTo));
            actionContext.HttpContext.Response.Redirect(uri.AbsoluteUri, Parameters.Permanent, Parameters.PreserveMethod);
        }
    }
}
