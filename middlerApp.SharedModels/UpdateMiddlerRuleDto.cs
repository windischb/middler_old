using System;
using System.Collections.Generic;
using middler.Common.SharedModels.Models;

namespace middler.Hosting.Models {
    public class UpdateMiddlerRuleDto {

        public string Name { get; set; }


        public List<string> Scheme { get; set; } = new List<string>();


        public string Hostname { get; set; }


        public string Path { get; set; }


        public List<string> HttpMethods { get; set; } = new List<string>();


        public List<MiddlerRulePermission> Permissions { get; set; } = new List<MiddlerRulePermission>();


        public List<MiddlerAction> Actions { get; set; } = new List<MiddlerAction>();

        public bool Enabled { get; set; }

        public decimal Order { get; set; }


    }
}
