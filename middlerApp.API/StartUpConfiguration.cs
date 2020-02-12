using System;
using Reflectensions.ExtensionMethods;

namespace middlerApp.API
{
    public class StartUpConfiguration
    {
        public string ListeningIP { get; set; } = "0.0.0.0";
        public int? HttpPort { get; set; } = 80;
        public int? HttpsPort { get; set; } = 443;
        public string HttpsCertPath { get; set; } = "localhost.pfx";
        public string HttpsCertPassword { get; set; } = "ABC12abc";
        public string InstanceIdentifier { get; set; } = Environment.MachineName;

        public StartUpAdminConfiguration AdminSettings { get; } = new StartUpAdminConfiguration();

        public void SetAdminSettings()
        {
            AdminSettings.ListeningIP = AdminSettings.ListeningIP?.Trim().ToNull() ?? ListeningIP;
            AdminSettings.HttpsPort = AdminSettings.HttpsPort != 0 ? AdminSettings.HttpsPort : 4444;
            AdminSettings.HttpsCertPath = AdminSettings.HttpsCertPath?.Trim().ToNull() ?? HttpsCertPath;
            AdminSettings.HttpsCertPassword = AdminSettings.HttpsCertPassword?.Trim().ToNull() ?? HttpsCertPassword;
        }
    }

    public class StartUpAdminConfiguration
    {
        public string ListeningIP { get; set; } = "127.0.0.1";
        public int HttpsPort { get; set; } = 4444;
        public string HttpsCertPath { get; set; }
        public string HttpsCertPassword { get; set; }

        
    }



}
