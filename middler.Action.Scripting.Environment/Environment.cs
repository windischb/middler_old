using middler.Scripting.HttpCommand;
using middler.Scripting.SmtpCommand;
using middler.Scripting.TemplateCommand;
using middler.Scripting.Variables;
using middler.Variables.LiteDB;

namespace middler.Scripting
{
    public class Environment
    {
        public Http Http => new Http();
        public TaskHelper Task => new TaskHelper();

        public VariableCommand Variables;

        public Smtp Smtp => new Smtp();

        public MTemplate Template => new MTemplate();

        public Environment(VariableStore variableStore)
        {
            Variables = new VariableCommand(variableStore);    
        }
    }
}
