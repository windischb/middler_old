using middler.Scripting.HttpCommand;
using middler.Scripting.SmtpCommand;
using middler.Scripting.TemplateCommand;
using middler.Scripting.Variables;
using middlerApp.Data;

namespace middler.Scripting
{
    public class Environment
    {
        public Http Http => new Http();
        public TaskHelper Task => new TaskHelper();

        public VariableCommand Variables;

        public Smtp Smtp => new Smtp();

        public MTemplate Template => new MTemplate();

        public Environment(VariablesRepository variableStore)
        {
            Variables = new VariableCommand(variableStore);    
        }
    }
}
