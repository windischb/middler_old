namespace middler.Action.Scripting.Shared
{
    public interface IScriptContextResponse
    {
        int? StatusCode { get; set; }

        object Body { get; set; }
    }
}