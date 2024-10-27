namespace ControlSharp.Api.Config.Model;

public class Access
{
    public required Guid Id { get; set; }
    public required Asset Asset { get; set; }
    public required ApiKey Key { get; set; }
}