using middler.Action.Scripting.Shared;

namespace middler.Action.Scripting.Models
{
    public class ScriptContextResponse: IScriptContextResponse
    {
        public int? StatusCode { get; set; }
        public object Body { get; set; }
    }
}