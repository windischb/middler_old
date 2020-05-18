using middler.Scripting.HttpCommand;
using middler.Scripting.Variables;
using middler.Variables.LiteDB;

namespace middler.Scripting
{
    public class Environment
    {
        public Http Http => new Http();
        public TaskHelper Task => new TaskHelper();

        public VariableCommand Variables;

        public Environment(VariableStore variableStore)
        {
            Variables = new VariableCommand(variableStore);    
        }
    }
}
