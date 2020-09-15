using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;
using middlerApp.API.Helper;
using middlerApp.API.IDP.DtoModels;
using middlerApp.API.IDP.Services;
using middlerApp.API.IDP.Storage.Entities;
using Client = middlerApp.API.IDP.Storage.Entities.Client;

namespace middlerApp.API.IDP
{
    public class DefaultResourcesManager
    {
        public IDPDbContext DbContext { get; }



        public DefaultResourcesManager(IDPDbContext dbContext)
        {
            DbContext = dbContext;
            
        }


        public void EnsureAllResourcesExists()
        {

            EnsureAdminClientExists();
        }

        public void EnsureAdminClientExists()
        {

            var adminClient =  DbContext.Clients
                .Include(c => c.RedirectUris)
                .FirstOrDefault(c => c.Id == IdpDefaultIdentifier.IdpClient);

            if (adminClient != null)
            {
                UpdateAdminClient(adminClient);
                return;
            }
               

            var client = new Client();
            client.Id = IdpDefaultIdentifier.IdpClient;
            client.ClientId = "mAdmin";
            client.ClientName = "middler Admin UI";
            client.Description = "Administration UI for middler & IdentityServer";
            client.Enabled = true;
            client.RequireClientSecret = false;
            client.AllowedGrantTypes = new List<ClientGrantType>
            {
                new ClientGrantType
                {
                    ClientId = client.Id,
                    GrantType = "authorization_code"
                }
            };
            client.AccessTokenType = (int)AccessTokenType.Reference;
            SetRedirectUris(client);


            DbContext.Clients.Add(client);
            DbContext.SaveChanges();

        }

        private void UpdateAdminClient(Client client)
        {

            SetRedirectUris(client);
            DbContext.SaveChanges();

        }


        private void SetRedirectUris(Client client)
        {
            var uris = client.RedirectUris.Select(u => u.RedirectUri).ToList();
            var idpUri = GenerateIdpRedirectUri();

            if (!uris.Contains(idpUri))
            {
                client.RedirectUris.Add(new ClientRedirectUri
                {
                    ClientId = client.Id,
                    RedirectUri = idpUri
                });
            }

            var admUri = GenerateAdminRedirectUri();
            if (!uris.Contains(admUri))
            {
                client.RedirectUris.Add(new ClientRedirectUri
                {
                    ClientId = client.Id,
                    RedirectUri = admUri
                });
            }
        }
        private string GenerateIdpRedirectUri()
        {
            var conf = Static.StartUpConfiguration.IdpSettings;
            var idpListenIp = IPAddress.Parse(conf.ListeningIP);
            var isLocalhost = IPAddress.IsLoopback(idpListenIp) || idpListenIp.ToString() == IPAddress.Any.ToString();

            if (isLocalhost)
            {
                return conf.HttpsPort == 443 ? $"https://localhost" : $"https://localhost:{conf.HttpsPort}";
            }
            else
            {
                return conf.HttpsPort == 443
                    ? $"https://{conf.ListeningIP}"
                    : $"https://{conf.ListeningIP}:{conf.HttpsPort}";
            }
        }

        private string GenerateAdminRedirectUri()
        {
            var conf = Static.StartUpConfiguration.AdminSettings;
            var idpListenIp = IPAddress.Parse(conf.ListeningIP);
            var isLocalhost = IPAddress.IsLoopback(idpListenIp) || idpListenIp.ToString() == IPAddress.Any.ToString();

            if (isLocalhost)
            {
                return conf.HttpsPort == 443 ? $"https://localhost" : $"https://localhost:{conf.HttpsPort}";
            }
            else
            {
                return conf.HttpsPort == 443
                    ? $"https://{conf.ListeningIP}"
                    : $"https://{conf.ListeningIP}:{conf.HttpsPort}";
            }
        }
    }
}
