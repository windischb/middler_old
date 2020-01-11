using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using middler.Common;
using middler.Common.Interfaces;
using middler.Core.Models;

namespace middler.Core
{
    public class MiddlerActionHelper : IMiddlerActionHelper
    {
        private MiddlerActionContext MiddlerActionContext { get; }
        private IMiddlerOptions MiddlerOptions { get; }

        public MiddlerActionHelper(IMiddlerOptions middlerOptions, MiddlerActionContext context)
        {
            MiddlerActionContext = context;
            MiddlerOptions = middlerOptions;
        }

        public AutoStream CreateDefaultAutoStream()
        {
            return new AutoStream(MiddlerOptions.AutoStreamDefaultMemoryThreshold, MiddlerActionContext.HttpContext.RequestAborted);
        }

        public string BuildPathFromRoutData(string template)
        {
            string ProcessHtmlTag(Match m)
            {
                string part = m.Groups["part"].Value;

                if (MiddlerActionContext.Request.RouteData.ContainsKey(part))
                {
                    return MiddlerActionContext.Request.RouteData[part]?.ToString();
                }

                return null;
            }

            Regex regex = new Regex("{(?<part>([a-zA-Z0-9]*))}");
            string cleanString = regex.Replace(template, ProcessHtmlTag);
            return cleanString;
        }
    }
}
