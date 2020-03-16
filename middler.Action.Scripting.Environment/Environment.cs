using middler.Scripting.HttpCommand;

namespace middler.Scripting
{
    public class Environment
    {
        public Http Http => new Http();
        public AsyncTaskHelper Async => new AsyncTaskHelper();
    }
}
