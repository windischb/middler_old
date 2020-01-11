using System;
using System.Collections.Generic;
using middler.Common.Models;

namespace middler.Common.Storage
{
    public class MiddlerRuleDbModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public decimal Order { get; set; }
        public List<string> Scheme { get; set; } = new List<string>();
        public string Hostname { get; set; }
        public string Path { get; set; }
        public List<string> HttpMethods { get; set; } = new List<string>();
        //public List<MiddlerRulePermission> Permissions { get; set; } = new List<MiddlerRulePermission>();
        public List<MiddlerAction> Actions { get; set; } = new List<MiddlerAction>();

    }


    public static class MiddlerRuleDbModelExtensions {
        public static MiddlerRule ToMiddlerRule(this MiddlerRuleDbModel dbModel) {
            var rule = new MiddlerRule();
            rule.Scheme = dbModel.Scheme;
            rule.Hostname = dbModel.Hostname;
            rule.Path = dbModel.Path;
            rule.HttpMethods = dbModel.HttpMethods;
            rule.Actions = dbModel.Actions;

            return rule;
        }
    }

}
