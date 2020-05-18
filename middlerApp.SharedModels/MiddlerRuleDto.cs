using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using middler.Common;
using middler.Common.SharedModels.Models;

namespace middler.Hosting.Models {
    public class MiddlerRuleDto {

        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<string> Scheme { get; set; } = new List<string>();
        public string Hostname { get; set; }
        public string Path { get; set; }
        public List<string> HttpMethods { get; set; } = new List<string>();
        public List<MiddlerRulePermission> Permissions { get; set; } = new List<MiddlerRulePermission>();
        public List<MiddlerActionDto> Actions { get; set; } = new List<MiddlerActionDto>();

        public bool Enabled { get; set; }

        public decimal Order { get; set; }
    }
}
