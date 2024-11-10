using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;

namespace ControlSharp.Client.Worker;

public class SingalRWorker : IHostedService
{
    private readonly ILogger<SingalRWorker> _logger;
    private readonly IConfiguration _configuration;
    private readonly HubConnection _assetHub;

    public SingalRWorker(ILogger<SingalRWorker> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
        _assetHub = new HubConnectionBuilder()
            .WithUrl("https://localhost:7175/asset", config =>
            {
                config.AccessTokenProvider = () => Task.FromResult(_configuration.GetValue<string>("token"));
            })
            .Build();

        _assetHub.On("cool", (string message) =>
        {
            _logger.LogInformation(message);
        });
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {      
        await _assetHub.StartAsync(cancellationToken);
        
        while (true)
        {
            await Task.Delay(3000, cancellationToken);
            await _assetHub.InvokeAsync("SendMessageToAll", "Hello World!", cancellationToken);
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        
    }
}