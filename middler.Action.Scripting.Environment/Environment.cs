using middler.Scripting.HttpCommand;
using middler.Scripting.Variables;
using middler.Variables;

namespace middler.Scripting
{
    public class Environment
    {
        public Http Http => new Http();
        public TaskHelper Task => new TaskHelper();

        public VariableCommand Variables;

        public Environment(IVariablesStore variableStore)
        {
            Variables = new VariableCommand(variableStore);    
        }
    }
}
