using System;
using System.Collections.Generic;
using System.Text;

namespace middlerApp.Data
{
    public class EndpointRuleEntity
    {
        public Guid Id { get; set; }
        public decimal Order { get; set; }
        
        public string Name { get; set; }
        public bool Enabled { get; set; }
        
        public string Scheme { get; set; }
        public string Hostname { get; set; }
        public string Path { get; set; }
        public string HttpMethods { get; set; }
        //public List<MiddlerRulePermission> Permissions { get; set; } = new List<MiddlerRulePermission>();
        public List<EndpointActionEntity> Actions { get; set; } = new List<EndpointActionEntity>();






    }
}
