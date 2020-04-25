using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using middler.Common;
using middler.Common.Interfaces;
using middler.Common.SharedModels.Models;

namespace middler.Core
{
    public class ActionHelper: IActionHelper
    {
        private MiddlerRouteData _routeData;

        public ActionHelper(IMiddlerRequestContext middlerRequestContext)
        {
            _routeData = middlerRequestContext.RouteData;
        }

        public string BuildPathFromRoutData(string template)
        {
            string ProcessHtmlTag(Match m)
            {
                string part = m.Groups["part"].Value;

                if (_routeData.ContainsKey(part))
                {
                    return _routeData[part]?.ToString();
                }

                return null;
            }

            Regex regex = new Regex("{(?<part>([a-zA-Z0-9]*))}");
            string cleanString = regex.Replace(template, ProcessHtmlTag);
            return cleanString;
        }
    }
}
