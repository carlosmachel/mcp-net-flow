using System.Net.Http.Headers;
using MCPServer.Tools;

var builder = WebApplication.CreateBuilder(args);
const string serverUrl = "http://localhost:7071/";

builder.Services.AddMcpServer()
    .WithTools<WeatherTools>()
    .WithHttpTransport();
    
// Configure HttpClientFactory for weather.gov API
builder.Services.AddHttpClient("WeatherApi", client =>
{
    client.BaseAddress = new Uri("https://api.weather.gov");
    client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("weather-tool", "1.0"));
});

var app = builder.Build();

app.MapMcp();

app.Run(serverUrl);