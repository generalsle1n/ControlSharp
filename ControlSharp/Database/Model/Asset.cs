using System.Net;

namespace ControlSharp.Config.Model;

public class Asset
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required DateTimeOffset Created { get; set; }
    public DateTimeOffset LastOnline { get; set; }
    public IPAddress Ip { get; set; }
}