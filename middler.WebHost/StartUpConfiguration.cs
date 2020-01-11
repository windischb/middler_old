using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace middler.WebHost
{
    public class StartUpConfiguration
    {
        [Description("Limit API to listen only on specified IP's")]
        public string ListenIP { get; set; } = "0.0.0.0";

        [Description("API Listening HTTPS Port, set to 0 to turn off HTTPS")]
        public int? HttpPort { get; set; } = 80;

        [Description("API Listening HTTPS Port, set to 0 to turn off HTTPS")]
        public int? HttpsPort { get; set; } = 443;

        [Description("Path to the SSL Certificate used for HTTPS")]
        public string HttpsCertPath { get; set; } = "localhost.pfx";

        [Description("Password for the SSL Certificate used by HTTPS")]
        public string HttpsCertPassword { get; set; } = "ABC12abc";

        [Description("Unique Api Identifier, used for identifying one specific API Instance")]
        public string InstanceIdentifier { get; set; } = Environment.MachineName;

    }

}
