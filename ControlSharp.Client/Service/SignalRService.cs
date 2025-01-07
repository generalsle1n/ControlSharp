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
    private HubConnection _mainHub;
    private const int TimeOut = 1000;
    private const int BufferSize = 1024;
    private string _connectionId;
    private HttpClient _client;

    public SignalRService(ILogger<SignalRService> logger, IConfiguration configuration, HttpClient client)
    {
        _logger = logger;
        _configuration = configuration;
        _client = client;
        
        ConfigureHub();
    }
    
    private void ConfigureHub()
    {
        string Server = _configuration.GetValue<string>("server");
        IHubConnectionBuilder AssetHubBuilder = new HubConnectionBuilder()
            .WithUrl($"{Server}/login")
            .AddMessagePackProtocol()
            .WithKeepAliveInterval(TimeSpan.FromSeconds(30))
            .WithStatefulReconnect()
            .WithAutomaticReconnect();
        
        AssetHubBuilder.Services.Configure<HubConnectionOptions>(config =>
        {
            config.StatefulReconnectBufferSize = BufferSize;
        });
        
        _assetHub = AssetHubBuilder.Build();
        
        _assetHub.On("DestroyAssetAsync", () =>
        {
            Environment.Exit(0);
        });

        _assetHub.On("CreateConnectingToMain", (string ID) =>
        {
            _connectionId = ID;
        });
    }

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
    private async Task ConfigureMainConnectionAsync(CancellationToken cancellationToken)
    {
        string Token = await GetTokenAsync(cancellationToken);
        string Server = _configuration.GetValue<string>("server");
        IHubConnectionBuilder AssetHubBuilder = new HubConnectionBuilder()
            .WithUrl($"{Server}/{_connectionId}")
            .AddMessagePackProtocol()
            .WithKeepAliveInterval(TimeSpan.FromSeconds(30))
            .WithStatefulReconnect()
            .WithAutomaticReconnect();

        AssetHubBuilder.Services.Configure<HubConnectionOptions>(config =>
        {
            config.StatefulReconnectBufferSize = BufferSize;
        });

        _mainHub = AssetHubBuilder.Build();
    }
}