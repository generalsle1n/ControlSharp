using Microsoft.Extensions.Configuration;
using Projects;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<ProjectResource> ControlApi = builder.AddProject<ControlSharp_Api>("ControlSharp-Api")
    .WithReplicas(builder.Configuration.GetValue<int>("Replicas"));

IResourceBuilder<ProjectResource> ControlUi = builder.AddProject<ControlSharp_Ui>("ControlSharp-Ui")
    .WithReplicas(builder.Configuration.GetValue<int>("Replicas"))
    .WithReference(ControlApi)
    .WaitFor(ControlApi);

builder.AddProject<ControlSharp_Client>("ControlSharp-Client")
    .WaitFor(ControlUi)
    .WithEnvironment("server", ControlApi.GetEndpoint("https"));

DistributedApplication app = builder.Build();
await app.RunAsync();