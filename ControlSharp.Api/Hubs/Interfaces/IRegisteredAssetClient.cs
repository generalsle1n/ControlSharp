namespace ControlSharp.Api.Hubs.Interfaces;

public interface IRegisteredAssetClient
{
    public Task ExecuteBinary(string message);
}