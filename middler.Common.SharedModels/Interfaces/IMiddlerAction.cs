namespace middler.Common.SharedModels.Interfaces
{
    public interface IMiddlerAction {
        bool Terminating { get; set; }
        bool WriteStreamDirect { get; set; }
        string ActionType { get; }

    }
}