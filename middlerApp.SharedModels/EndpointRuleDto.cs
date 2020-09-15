using System;
using System.Collections.Generic;

namespace middlerApp.SharedModels {
    public class EndpointRuleDto {

        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<string> Scheme { get; set; } = new List<string>();
        public string Hostname { get; set; }
        public string Path { get; set; }
        public List<string> HttpMethods { get; set; } = new List<string>();
        public List<EndpointActionDto> Actions { get; set; } = new List<EndpointActionDto>();
        public bool Enabled { get; set; }

        public decimal Order { get; set; }
    }
}
