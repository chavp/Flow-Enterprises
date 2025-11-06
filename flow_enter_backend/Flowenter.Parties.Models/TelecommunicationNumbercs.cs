namespace Flowenter.Parties.Models;

public sealed class TelecommunicationNumber : ContactMechanism
{
    public string CountryCode { get; set; }
    public string AreaCode { get; set; }
    public string ContactNumber { get; set; }
    public string? AskForName { get; set; }
}
