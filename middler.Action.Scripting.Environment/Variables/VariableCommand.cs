using System;
using System.Linq;
using middler.Common.Variables;
using middler.Common.Variables.HelperClasses;
using middler.Variables.LiteDB;
using Reflectensions.ExtensionMethods;

namespace middler.Scripting.Variables
{
    public class VariableCommand
    {
        private readonly VariableStore _variablesStore;

        public VariableCommand(VariableStore variablesStore)
        {
            _variablesStore = variablesStore;
        }

        public TreeNode GetVariable(string path)
        {
            path = path.Replace(".", "/");

            string parent = null;
            string name = path;
            if (path.Contains("/"))
            {
                var parts = path.Split('/');
                parent = String.Join('/', parts.Take(parts.Length - 1));
                name = parts.Last();
            }

            return _variablesStore.GetVariable(parent, name);
        }

        public object GetAny(string path)
        {
            var variable = this.GetVariable(path);
            return variable.Content;
        }

        public string GetString(string path)
        {
            var variable = this.GetVariable($"{path}");
            return (string)variable.Content;
        }

        public decimal GetNumber(string path)
        {
            var variable = this.GetVariable($"{path}");
            return (decimal)variable.Content;
        }

        public bool GetBoolean(string path)
        {
            var variable = this.GetVariable($"{path}");
            return (bool)variable.Content;
        }

        public SimpleCredentials GetCredential(string path)
        {
            var variable = this.GetVariable($"{path}.credential");
            return (SimpleCredentials)variable.Content;
        }
    }
}
