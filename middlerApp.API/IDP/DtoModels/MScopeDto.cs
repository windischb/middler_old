
using System;
using System.Collections.Generic;
using middlerApp.API.IDP.Storage.Entities;

namespace middlerApp.API.IDP.DtoModels
{
    public class MScopeDto
    {
        public Guid Id { get; set; }
        public bool Enabled { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public bool Required { get; set; }
        public bool Emphasize { get; set; }
        public bool ShowInDiscoveryDocument { get; set; }
        public string Type { get; set; }
        public List<string> UserClaims { get; set; }
        public List<object> Properties { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public bool NonEditable { get; set; }
    }
}
