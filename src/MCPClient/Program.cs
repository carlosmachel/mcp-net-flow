using Microsoft.Extensions.Logging;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;

const string serverUrl = "http://localhost:7071/";

var httpClient = new HttpClient();

var consoleLoggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddConsole().SetMinimumLevel(LogLevel.Information);
});

var transport = new SseClientTransport(new SseClientTransportOptions
{
    Endpoint = new Uri(serverUrl),
    Name = "Secure Weather Client with Entra ID"
}, httpClient, consoleLoggerFactory);


var client = await McpClientFactory.CreateAsync(transport, loggerFactory: consoleLoggerFactory);

var logger = consoleLoggerFactory.CreateLogger<Program>();

var tools = await client.ListToolsAsync();
if (tools.Count == 0)
{
    logger.LogInformation("No tools available on the server");
    return;
}

logger.LogInformation("Found {ToolsCount} tools on the server:", tools.Count);
foreach (var tool in tools)
{
    logger.LogInformation("  - {ToolName}: {ToolDescription}", tool.Name, tool.Description);
}

if (tools.Any(t => t.Name == "get_alerts"))
{
    logger.LogInformation("Calling get_alerts tool...");

    var result = await client.CallToolAsync(
        "get_alerts",
        new Dictionary<string, object?> { { "state", "WA" } }
    );

    logger.LogInformation("Result: " + ((TextContentBlock)result.Content[0]).Text);
}

// Exemplo de chamada para outro tool se existir
if (tools.Any(t => t.Name == "get_forecast"))
{
    logger.LogInformation("Calling get_forecast tool...");

    var result = await client.CallToolAsync(
        "get_forecast",
        new Dictionary<string, object?> 
        { 
            { "latitude", 47.6062 }, 
            { "longitude", -122.3321 } // Seattle coordinates
        }
    );

    logger.LogInformation("Forecast: " + ((TextContentBlock)result.Content[0]).Text);
}