using System;
using System.Collections.Generic;
using System.Text;

namespace middlerApp.SharedModels
{
    public class EndpointRuleListActionDto
    {
        public Guid Id { get; set; }
        public virtual bool Enabled { get; set; }
        public string ActionType { get; set; }
    }
}
