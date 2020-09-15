using System;
using middlerApp.API.Helper;
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
        
        
        public StartUpAdminConfiguration AdminSettings { get; } = new StartUpAdminConfiguration();

        public StartUpIdpConfiguration IdpSettings { get; } = new StartUpIdpConfiguration();

        //public EndpointRulesConfiguration EndpointRulesSettings { get; } = new EndpointRulesConfiguration();

        //public GlobalVariablesConfiguration GlobalVariablesSettings { get; } = new GlobalVariablesConfiguration();

        public StartUpConfiguration SetDefaultSettings()
        {
            AdminSettings.ListeningIP = AdminSettings.ListeningIP?.Trim().ToNull() ?? ListeningIP;
            AdminSettings.HttpsPort = AdminSettings.HttpsPort != 0 ? AdminSettings.HttpsPort : 4444;
            AdminSettings.HttpsCertPath = AdminSettings.HttpsCertPath?.Trim().ToNull() ?? HttpsCertPath;
            AdminSettings.HttpsCertPassword = AdminSettings.HttpsCertPassword?.Trim().ToNull() ?? HttpsCertPassword;

            IdpSettings.ListeningIP = IdpSettings.ListeningIP?.Trim().ToNull() ?? ListeningIP;
            IdpSettings.HttpsPort = IdpSettings.HttpsPort != 0 ? IdpSettings.HttpsPort : 4444;
            IdpSettings.HttpsCertPath = IdpSettings.HttpsCertPath?.Trim().ToNull() ?? HttpsCertPath;
            IdpSettings.HttpsCertPassword = IdpSettings.HttpsCertPassword?.Trim().ToNull() ?? HttpsCertPassword;

            //EndpointRulesSettings.DbFilePath = EndpointRulesSettings.DbFilePath?.Trim().ToNull() ?? "DefaultStorage/rules.db";

            //GlobalVariablesSettings.DbFilePath = GlobalVariablesSettings.DbFilePath?.Trim().ToNull() ?? "DefaultStorage/variables.db";

            return this;
        }
    }

    public class StartUpAdminConfiguration
    {
        public string ListeningIP { get; set; } = "0.0.0.0";
        public int HttpsPort { get; set; } = 4444;
        public string HttpsCertPath { get; set; }
        public string HttpsCertPassword { get; set; }
        public string WebRoot { get; set; } = "AdminUI";

    }

    public class StartUpIdpConfiguration
    {
        public string ListeningIP { get; set; } = "0.0.0.0";
        public int HttpsPort { get; set; } = 4445;
        public string HttpsCertPath { get; set; }
        public string HttpsCertPassword { get; set; }
        public string WebRoot { get; set; } = "IdentityUI";

    }

    //public class EndpointRulesConfiguration
    //{
    //    public string DbFilePath { get; set; }
    //}

    //public class GlobalVariablesConfiguration
    //{
    //    public string DbFilePath { get; set; }
    //}




}
