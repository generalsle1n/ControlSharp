using System.Net;
using ControlSharp.Client.Helper;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;

namespace ControlSharp.Client.Worker;

public class SignalRWorker : IHostedService
{
    private readonly ILogger<SignalRWorker> _logger;
    private readonly IConfiguration _configuration;
    private HubConnection _assetHub;

    public SignalRWorker(ILogger<SignalRWorker> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
        
        ConfigureHub();
    }
    
    private void ConfigureHub()
    {
        string Server = _configuration.GetValue<string>("url");
        _assetHub = new HubConnectionBuilder()
            .WithUrl($"{Server}/asset", config =>
            {
                
            })
            .AddMessagePackProtocol()
            .Build();
        
        _assetHub.On("ExecuteBinary", (string message) =>
        {
            _logger.LogInformation(message);
        });
    } 
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {      
        await _assetHub.StartAsync(cancellationToken);
        await _assetHub.InvokeAsync("Register", await DeviceIDGenerator.GenerateAsync(), Dns.GetHostName(), cancellationToken);
        while (true)
        {
            await Task.Delay(3000, cancellationToken);
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        
    }
}