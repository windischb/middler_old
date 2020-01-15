using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using middlerApp.Server.Helper;
using Reflectensions.ExtensionMethods;

namespace middlerApp.Server
{
    //public static class ListeningEndpointManager
    //{

    //    public static List<ListenEndpoint> RegisteredEndpoints { get; private set; } = new List<ListenEndpoint>();

    //    private static ListenEndpoint FindMatchingEndpoint(ListenEndpoint endpoint)
    //    {
    //        return RegisteredEndpoints.FirstOrDefault(e =>
    //            e.ListeningIP.Equals(endpoint.ListeningIP) &&
    //            e.Port.Equals(endpoint.Port) &&
    //            e.Https.Equals(endpoint.Https) &&
    //            e.HttpsCertPath?.Equals(endpoint.HttpsCertPath) != false &&
    //            e.HttpsCertPassword?.Equals(endpoint.HttpsCertPassword) != false
    //        );

    //    }

    //    private static IEnumerable<ListenEndpoint> BuildListeningEndpointsFromConfig(IHttpEndpointConfiguration config, bool admin)
    //    {
    //        var listeningEndpoints = new List<ListenEndpoint>();

            

    //        if (config.HttpPort.HasValue && config.HttpPort.Value != 0)
    //        {
    //            var httpEndp = new ListenEndpoint()
    //            {
    //                ListeningIP = IPAddressRange.TryParse(config.ListeningIP, out var ip)
    //                    ? ip.Begin
    //                    : IPAddress.Loopback,
    //                Port = config.HttpPort.Value,
    //                Https = false,
    //                IsAdminAreaRequest = admin
    //            };
    //            if (config is StartUpAdminConfiguration aConf)
    //            {
    //                httpEndp.HostHeader = aConf.HostHeader?.Trim().ToNull();
    //            }
    //            listeningEndpoints.Add(httpEndp);
    //        }

    //        if (config.HttpsPort.HasValue && config.HttpsPort.Value != 0)
    //        {
    //            var httpsEndp = new ListenEndpoint()
    //            {
    //                ListeningIP = IPAddressRange.TryParse(config.ListeningIP, out var ip)
    //                    ? ip.Begin
    //                    : IPAddress.Loopback,
    //                Port = config.HttpsPort.Value,
    //                Https = true,
    //                HttpsCertPath = config.HttpsCertPath,
    //                HttpsCertPassword = config.HttpsCertPassword,
    //                IsAdminAreaRequest = admin
    //            };
    //            if (config is StartUpAdminConfiguration aConf)
    //            {
    //                httpsEndp.HostHeader = aConf.HostHeader?.Trim().ToNull();
    //            }
    //            listeningEndpoints.Add(httpsEndp);
    //        }

    //        return listeningEndpoints;
    //    }

    //    public static void BuildListeningEndpoints(StartUpConfiguration config)
    //    {

    //        config.AdminSettings.ListeningIP = config.AdminSettings.ListeningIP?.Trim().ToNull() ?? config.ListeningIP;
    //        config.AdminSettings.HttpsPort = config.AdminSettings.HttpsPort.HasValue && config.AdminSettings.HttpsPort.Value != 0 ? config.AdminSettings.HttpsPort.Value : config.HttpsPort;
    //        config.AdminSettings.HttpsCertPath = config.AdminSettings.HttpsCertPath?.Trim().ToNull() ?? config.HttpsCertPath;
    //        config.AdminSettings.HttpsCertPassword = config.AdminSettings.HttpsCertPassword?.Trim().ToNull() ?? config.HttpsCertPassword;
            
    //        var defaultListeningEndpoints = BuildListeningEndpointsFromConfig(config, false);
    //        var adminListeningEndpoints = BuildListeningEndpointsFromConfig(config.AdminSettings, true);
    //        RegisteredEndpoints.AddRange(adminListeningEndpoints);

    //        foreach (var defaultListeningEndpoint in defaultListeningEndpoints)
    //        {
    //            var exist = FindMatchingEndpoint(defaultListeningEndpoint);
    //            if (exist == null)
    //            {
    //                RegisteredEndpoints.Add(defaultListeningEndpoint);
    //            }
    //        }


    //        RegisteredEndpoints = RegisteredEndpoints.Where(e => e != null).ToList();
    //    }

        
    //}

    //public class ListenEndpoint
    //{
    //    public IPAddress ListeningIP { get; set; }
    //    public string HostHeader { get; set; }
    //    public int Port { get; set; }
    //    public bool Https { get; set; }
    //    public string HttpsCertPath { get; set; }
    //    public string HttpsCertPassword { get; set; }
    //    public bool IsAdminAreaRequest { get; set; }
    //}
}
