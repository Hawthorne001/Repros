using System.Diagnostics;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateSlimBuilder(args);

        var useAzureMonitor = Config.AzureMonitorEnabled(builder.Configuration);
        var useAspNetCoreInstrumentation = Config.AspNetCoreInstrumentationEnabled(builder.Configuration);

var otel = builder.Services.AddOpenTelemetry().WithTracing(x =>
{
    if (useAspNetCoreInstrumentation)
    {
        // This is (almost) well-behaved and does not cause concurrency issues,
        // and simply mutates the existing "Microsoft.AspNetCore.Hosting.HttpRequestIn" activity
    
        // DisplayName: "Microsoft.AspNetCore.Hosting.HttpRequestIn",
        // OperationName: "POST <endpoint>" OR "POST", (inconsistent)
        // 7 tags ("server.address", "http.request.method", "url.scheme", "url.path", "network.protocol.version", "http.route", and "http.response.status_code").
        x.AddAspNetCoreInstrumentation();
    }
});

if (useAzureMonitor)
{
    // this ain't well-behaved and causes concurrency issues and 3 "Microsoft.AspNetCore.Hosting.HttpRequestIn" activities.
    
    // 1 with DisplayName: "Microsoft.AspNetCore.Hosting.HttpRequestIn",
    // OperationName: "Microsoft.AspNetCore.Hosting.HttpRequestIn",
    // and no tags.
    
    // 1 with
    // DisplayName: "Microsoft.AspNetCore.Hosting.HttpRequestIn",
    // OperationName: "POST", and
    // 5 tags ("server.address", "http.request.method", "url.scheme", "url.path", and "network.protocol.version").
    //// Additional information: Disabling `AddAspNetCoreInstrumentation` in the `WithTracing` call removes this one for some reason,
    //// despite not being the one that adds it.
    
    // 1 with DisplayName: "Microsoft.AspNetCore.Hosting.HttpRequestIn",
    // OperationName: "POST /", and
    // 8 tags ("server.address", "http.request.method", "url.scheme", "url.path", "network.protocol.version", "http.route","http.response.status_code", and "_MS.ProcessedByMetricExtractors").
    
    // Get your shit together, Azure.. Caused me hours of issues :-|
    otel.UseAzureMonitor(options =>
    {
        // dummy instrumentation key to avoid exceptions
        options.ConnectionString = "InstrumentationKey=00000000-0000-0000-0000-000000000000";
    });
}

var app = builder.Build();

ActivitySource apiActivitySource = new("api-thingy");

app.MapPost(Config.EndpointName, (HttpContext httpContext) =>
{
    using var activity = apiActivitySource.StartActivity(Config.ActivityName)!;
    
    activity.AddTag("my-tag", "my-value");

    return "Hello World!";
});

app.Run();


public static class Config
{
    public const string EndpointName = "my-api-endpoint";
    public const string ActivityName = "my-api-endpoint-activity";
    public static bool AzureMonitorEnabled(IConfiguration config)
    {
        return config.GetValue<bool>("UseAzureMonitor");
    }

    public static bool AspNetCoreInstrumentationEnabled(IConfiguration configuration)
    {
        return configuration.GetValue<bool>("UseAspNetCoreInstrumentation");
    }
}