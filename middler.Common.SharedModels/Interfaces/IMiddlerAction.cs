namespace middler.Common.SharedModels.Interfaces
{
    public interface IMiddlerAction {
        bool ContinueAfterwards { get; set; }
        bool WriteStreamDirect { get; set; }
        string ActionType { get; }

    }
}