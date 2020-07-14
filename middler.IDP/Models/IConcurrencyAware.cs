namespace middler.IDP.Models
{
    public interface IConcurrencyAware
    {
        string ConcurrencyStamp { get; set; }
    }
}
