using System.Net;

namespace ControlSharp.Database.Identity.Model;

public class Asset
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Hash { get; set; }
    public required DateTimeOffset Created { get; set; }
    public DateTimeOffset? LastOnline { get; set; }
    public string? Ip { get; set; }
    public bool? Active { get; set; }
    public bool? Registered { get; set; }
    public bool? Banned { get; set; }
    public string? ConnectionId { get; set; }
}