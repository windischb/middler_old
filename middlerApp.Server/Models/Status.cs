using System;

namespace middlerApp.Server.Models
{
    public class Status
    {
        public string ServiceName { get; set; }
        public string Version { get; set; }
        public string ClientIp { get; set; }
        public string UserAgent { get; set; }
        public DateTime CurrentDateTime { get; set; }
        public string CurrentUser { get; set; }
        public string HostName { get; set; }
        public string[] ProxyServers { get; set; }
        public DateTime ServiceStart { get; set; }
        public TimeSpan ServiceRunningSince { get; set; }

        public string ContentRoot { get; set; }
        public string WebRoot { get; set; }

    }
}