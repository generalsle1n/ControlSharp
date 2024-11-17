using System.Net;
using ControlSharp.Client.Helper;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;

namespace ControlSharp.Client.Service;

public class SignalRService : BackgroundService
{
    private readonly ILogger<SignalRService> _logger;
    private readonly IConfiguration _configuration;
    private HubConnection _assetHub;

    public SignalRService(ILogger<SignalRService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
        
        ConfigureHub();
    }
    
    private void ConfigureHub()
    {
        string Server = _configuration.GetValue<string>("url");
        _assetHub = new HubConnectionBuilder()
            .WithUrl($"{Server}/login")
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
        
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await WaitForConnection(stoppingToken);
        await _assetHub.InvokeAsync("Register", await DeviceIDGenerator.GenerateAsync(), Dns.GetHostName(), stoppingToken);
        while (!stoppingToken.IsCancellationRequested)
        {
            
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }

            await Task.Delay(TimeOut, stoppingToken);
        }
    }

    private async Task WaitForConnection(CancellationToken cancellationToken)
    {
        bool Connected = false;
        while (Connected == false && cancellationToken.IsCancellationRequested == false)
        {
            try
            {
                await _assetHub.StartAsync(cancellationToken);
                Connected = true;
                break;
            }
            catch (HttpRequestException e)
            {
                await Task.Delay(TimeOut, cancellationToken);
            }
        }
    }
}