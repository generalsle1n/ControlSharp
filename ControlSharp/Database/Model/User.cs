namespace ControlSharp.Api.Config.Model;

public class User
{
    public required Guid Id { get; set; }
    public required string UserName { get; set; }
    public required string Password { get; set; }
    public required AccessRole Role { get; set; }
    public bool Active { get; set; }
    public required DateTimeOffset Created { get; set; }
    public DateTimeOffset? LastOnline { get; set; }
}