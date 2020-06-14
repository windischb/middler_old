using System;
using Newtonsoft.Json.Linq;

namespace middlerApp.Data
{
    public class EndpointActionEntity
    {

        public Guid Id { get; set; }
        public decimal Order { get; set; }
        public Guid EndpointRuleEntityId { get; set; }
        public bool Terminating { get; set; }
        public bool WriteStreamDirect { get; set; }
        public bool Enabled { get; set; }
        public string ActionType { get; set; }

        public JObject Parameters { get; set; } = new JObject();


    }
}
