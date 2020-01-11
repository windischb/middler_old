using System;
using System.Collections.Generic;

namespace middler.Common.Interfaces
{
    public interface IMiddlerActionRequest
    {

        string PathTemplate { get; set; }
        Uri Uri { get; set; }
        Dictionary<string, object> RouteData { get; set; }
        string ClientIp { get; set; }
        string[] ProxyServers { get; set; }

    }
}