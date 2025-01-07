namespace ControlSharp.Api.Hubs.Interfaces;

public interface IQuarantineAssetAction
{
    public Task CreateConnectingToMain(string ID);
    public Task DestroyAssetAsync();
}