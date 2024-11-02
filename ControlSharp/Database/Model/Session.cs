namespace ControlSharp.Api.Config.Model;

public class Session
{
    public required Guid Id { get; set; }
    public required SessionType Type{ get; set; }
    public ApiKey? ApiKey { get; set; }
    public Asset? Asset { get; set; }
    public User? User { get; set; }
}