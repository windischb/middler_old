using Microsoft.AspNetCore.Http;
using middler.Action.Scripting.Shared;

namespace middler.Action.Scripting.Models
{
    public class ScriptContext : IScriptContext
    {
        private readonly ScriptContextMethods _scriptContextMethods;
        
        public IScriptContextResponse Response { get; }
        public IScriptContextRequest Request { get; }

        public ScriptContext(HttpContext httpContext, ScriptContextMethods scriptContextMethods)
        {
            _scriptContextMethods = scriptContextMethods;
            Request = new ScriptContextRequest(httpContext);
            Response = new ScriptContextResponse();
        }

        public void SendResponse()
        {
            _scriptContextMethods.SendResponse?.Invoke();
        }
    }
}
