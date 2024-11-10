namespace ControlSharp.Api.Hubs.Interfaces;

public interface IQuarantineAssetHub
{
    public Task CreateConnectingToMain(string connectionId);
}