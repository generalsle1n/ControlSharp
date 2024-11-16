using Microsoft.Extensions.Configuration;
using Projects;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<ProjectResource> ControlApi = builder.AddProject<ControlSharp_Api>("ControlSharp-Api")
    .WithReplicas(builder.Configuration.GetValue<int>("Replicas"));

builder.AddProject<ControlSharp_Ui>("ControlSharp-Ui")
    .WithReplicas(builder.Configuration.GetValue<int>("Replicas"))
    .WithReference(ControlApi)
    .WaitFor(ControlApi);

builder.Build().Run();