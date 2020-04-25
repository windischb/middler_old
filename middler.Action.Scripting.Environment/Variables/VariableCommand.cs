using System;
using middler.Variables;
using middler.Variables.HelperClasses;
using Reflectensions.ExtensionMethods;

namespace middler.Scripting.Variables
{
    public class VariableCommand
    {
        private readonly IVariablesStore _variablesStore;

        public VariableCommand(IVariablesStore variablesStore)
        {
            _variablesStore = variablesStore;
        }

        public IVariable GetVariable(string path)
        {
            return _variablesStore.GetVariable(path);
        }

        public object GetAny(string path)
        {
            var variable = this.GetVariable(path);
            return variable.Content;
        }

        public string GetString(string path)
        {
            var variable = this.GetVariable($"{path}.string");
            return (string)variable.Content;
        }

        public decimal GetNumber(string path)
        {
            var variable = this.GetVariable($"{path}.number");
            return (decimal)variable.Content;
        }

        public bool GetBoolean(string path)
        {
            var variable = this.GetVariable($"{path}.boolean");
            return (bool)variable.Content;
        }

        public SimpleCredentials GetCredential(string path)
        {
            var variable = this.GetVariable($"{path}.credential");
            return (SimpleCredentials)variable.Content;
        }
    }
}
