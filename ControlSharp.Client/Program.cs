using Microsoft.AspNetCore.SignalR.Client;

string Url = "https://localhost:7175/api/0.1/assetHub";
var connection = new HubConnectionBuilder()
    .WithUrl(Url)
    .Build();


connection.On<string>("aaaaa", a =>
{
    Console.WriteLine("rec");
});

await connection.StartAsync();