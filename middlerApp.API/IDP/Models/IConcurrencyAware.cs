namespace middlerApp.API.IDP.Models
{
    public interface IConcurrencyAware
    {
        string ConcurrencyStamp { get; set; }
    }
}
