namespace ControlSharp.Api.Hubs.Interfaces;

public interface IAssetClient
{
    public Task SendMessageToAll(string message);
}