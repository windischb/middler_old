using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace middlerApp.API.IDP.Storage.Entities
{
    public class AuthorizationCode
    {
        public string Id { get; set; }

        public DateTime CreationTime { get; set; }

        public int Lifetime { get; set; }

        public string ClientId { get; set; }

        public ClaimsPrincipal Subject { get; set; }

        public bool IsOpenId { get; set; }

        public IEnumerable<string> RequestedScopes { get; set; }

        public string RedirectUri { get; set; }

        public string Nonce { get; set; }

        public string StateHash { get; set; }

        public bool WasConsentShown { get; set; }

        public string SessionId { get; set; }

        public string CodeChallenge { get; set; }

        public string CodeChallengeMethod { get; set; }

        public string Description { get; set; }

        public IDictionary<string, string> Properties { get; set; } = (IDictionary<string, string>)new Dictionary<string, string>();
    }
}
