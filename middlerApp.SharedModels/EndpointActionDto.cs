using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace middlerApp.SharedModels {
    public class EndpointActionDto {

        public Guid? Id { get; set; }
        public decimal Order { get; set; }
        public Guid EndpointRuleEntityId { get; set; }
        public virtual bool Terminating { get; set; }
        public virtual bool WriteStreamDirect { get; set; }
        public virtual bool Enabled { get; set; }
        public string ActionType { get; set; }
        public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();

    }
}
