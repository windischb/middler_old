using System;
using Microsoft.AspNetCore.Http;
using middler.Common;
using middler.Common.Interfaces;

namespace middler.Core.Models {


    public class MiddlerActionContext : IMiddlerActionContext {

        public HttpContext HttpContext { get; }
        public AutoStream ResponseBody { get; set; }

        public IMiddlerActionRequest Request { get; }
        public IMiddlerActionHelper Helper { get; }
        
        public MiddlerActionContext(IMiddlerOptions middlerOptions, HttpContext httpContext, MiddlerRuleMatch ruleMatch) {
            HttpContext = httpContext;
            Request = new MiddlerActionRequest(httpContext, ruleMatch);
            Helper = new MiddlerActionHelper(middlerOptions, this);
        }

    }
}
