using System;
using System.Collections.Generic;

namespace middler.Action.Scripting.Shared
{
    public interface IScriptContextRequest
    {
        string HttpMethod { get; }

        Uri Uri { get; }

        Dictionary<string, object> RouteData { get; set; }
        Dictionary<string, object> Headers { get; set; }
        Dictionary<string, string> QueryParameters { get; set; }

        string UserAgent { get;  }
        string ClientIp { get; }
        string[] ProxyServers { get; }

    }
}