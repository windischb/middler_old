

using System;
using System.Collections.Generic;

namespace middlerApp.API.IDP.DtoModels
{
    public class MApiResourceDto
    {
        public Guid Id { get; set; }
        public bool Enabled { get; set; } = true;
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string AllowedAccessTokenSigningAlgorithms { get; set; }
        public bool ShowInDiscoveryDocument { get; set; } = true;
        public List<object> Secrets { get; set; }
        public List<string> Scopes { get; set; }
        public List<string> UserClaims { get; set; }
        public List<object> Properties { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime? Updated { get; set; }
        public DateTime? LastAccessed { get; set; }
        public bool NonEditable { get; set; }
    }
}
