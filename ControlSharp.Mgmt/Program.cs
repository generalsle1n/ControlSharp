using Microsoft.Extensions.Configuration;
using Projects;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

string ControlSharpUiOICDIdToken = "ControlSharpUiOICDId";
string ControlSharpUiOICDSecretToken = "ControlSharpUiOICDSecret";
string ControlSharpUiServerToken = "ControlSharpUiServer";
string ControlSharpIdentityServerToken = "ControlSharpIdentityServer";
string ControlSharpUiOICDId = Guid.NewGuid().ToString();
string ControlSharpUiOICDSecret = Guid.NewGuid().ToString();

IResourceBuilder<ParameterResource> ControlSharpUiOICDIdParameter = builder.AddParameter(ControlSharpUiOICDIdToken, ControlSharpUiOICDId);
IResourceBuilder<ParameterResource> ControlSharpUiOICDSecretParameter = builder.AddParameter(ControlSharpUiOICDSecretToken, ControlSharpUiOICDSecret);

IResourceBuilder<ProjectResource> ControlIdentity = builder.AddProject<ControlSharp_Identity>("ControlSharp-Identity")
    .WithReplicas(builder.Configuration.GetValue<int>("Replicas"))
    .WithEnvironment(ControlSharpUiOICDIdToken, ControlSharpUiOICDIdParameter)
    .WithEnvironment(ControlSharpUiOICDSecretToken, ControlSharpUiOICDSecretParameter);

IResourceBuilder<ProjectResource> ControlApi = builder.AddProject<ControlSharp_Api>("ControlSharp-Api")
    .WithReplicas(builder.Configuration.GetValue<int>("Replicas"))
    .WithReference(ControlIdentity)
    .WaitFor(ControlIdentity);

IResourceBuilder<ProjectResource> ControlUi = builder.AddProject<ControlSharp_Ui>("ControlSharp-Ui")
    .WithReplicas(builder.Configuration.GetValue<int>("Replicas"))
    .WithReference(ControlApi)
    .WithReference(ControlIdentity)
    .WithEnvironment(ControlSharpUiOICDIdToken, ControlSharpUiOICDIdParameter)
    .WithEnvironment(ControlSharpUiOICDSecretToken, ControlSharpUiOICDSecretParameter)
    .WithEnvironment(ControlSharpIdentityServerToken, ControlIdentity.GetEndpoint("https"))
    .WaitFor(ControlApi)
    .WaitFor(ControlIdentity);

ControlIdentity.WithReference(ControlUi);
ControlIdentity.WithEnvironment(ControlSharpUiServerToken, ControlUi.GetEndpoint("https"));

builder.AddProject<ControlSharp_Client>("ControlSharp-Client")
    .WaitFor(ControlUi)
    .WithEnvironment("server", ControlApi.GetEndpoint("https"));



DistributedApplication app = builder.Build();
await app.RunAsync();