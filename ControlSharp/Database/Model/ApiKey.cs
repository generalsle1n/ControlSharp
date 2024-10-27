namespace ControlSharp.Api.Config.Model;

public class ApiKey
{
    public required Guid Id { get; set; }
    public required bool Active { get; set; }
    public required string Key { get; set; }
    public required DateTimeOffset Created { get; set; }
    public required AccessRole Role { get; set; }
}