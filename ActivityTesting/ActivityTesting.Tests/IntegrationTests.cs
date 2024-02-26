using System.Collections;
using System.Diagnostics;
using ActivityTesting.API;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace ActivityTesting.Tests;

public sealed class IntegrationTests
{
    private static readonly ActivitySource TestActivitySource;

    private static readonly List<Activity> Activities;

    // newing it up for simplicity's sake
    private static readonly IConfiguration Configuration =
        new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

    // Static constructor to set up the activity listener on startup
    static IntegrationTests()
    {
        Activities = [];
        TestActivitySource = new ActivitySource(nameof(MultiTest));
        var listener = new ActivityListener();
        listener.Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData;
        listener.ShouldListenTo = _ => true;
        listener.ActivityStarted = a =>
        {
            lock (Activities)
            {
                Activities.Add(a);
            }
        };
        ActivitySource.AddActivityListener(listener);
    }

    private static List<Activity> GetRelevantActivities(Activity activity)
    {
        lock (Activities)
        {
            return [..Activities.Where(x => x.TraceId == activity.TraceId).Except([activity])];
        }
    }

    [Theory]
    [ClassData(typeof(TestData))]
    public async Task MultiTest(int something)
    {
        // Arrange
        using var activity = TestActivitySource.StartActivity(nameof(MultiTest))!;
        var activityId = activity.Id;
        var httpContent = new StringContent("some content");
        httpContent.Headers.TryAddWithoutValidation("traceparent", activityId);

        var factory = new WebApplicationFactory<ApiAssemblyMarker>();
        using var client = factory.CreateClient();
        var uri = new Uri(client.BaseAddress!, Config.EndpointName); 

        // Act
        var response = await client.PostAsync(uri, httpContent);

        // Assert
        var relevantActivities = GetRelevantActivities(activity);

        var useAzureMonitor = Config.AzureMonitorEnabled(Configuration);
        var useAspNetCoreInstrumentation = Config.AspNetCoreInstrumentationEnabled(Configuration);

        const string requestInOperationName = "Microsoft.AspNetCore.Hosting.HttpRequestIn";
        const string requestOutOperationName = "System.Net.Http.HttpRequestOut";
        const string customTag = "my-tag";
        const string customValue = "my-value";

        switch (useAzureMonitor, useAspNetCoreInstrumentation)
        {
            case { useAzureMonitor: true, useAspNetCoreInstrumentation: true }:
                relevantActivities.Should().HaveCount(5);

                relevantActivities.Should().ContainSingle(x => x.OperationName == requestOutOperationName && x.DisplayName == requestOutOperationName);

                relevantActivities.Should().ContainSingle(x => x.OperationName == requestInOperationName && x.DisplayName == $"POST {Config.EndpointName}");
                relevantActivities.Should().ContainSingle(x => x.OperationName == requestInOperationName && x.DisplayName == "POST");
                relevantActivities.Should().ContainSingle(x => x.OperationName == requestInOperationName && x.DisplayName == requestInOperationName);

                relevantActivities.Should().ContainSingle(x => x.OperationName == Config.ActivityName)
                    .Which.TagObjects.Should().ContainKey(customTag).WhoseValue.Should().Be(customValue);

                break;
            case { useAzureMonitor: true, useAspNetCoreInstrumentation: false }:
                relevantActivities.Should().HaveCount(4);

                relevantActivities.Should().ContainSingle(x => x.OperationName == requestOutOperationName && x.DisplayName == requestOutOperationName);

                // For some reason it's inconsistent whether the display name is "POST /" or "POST"
                relevantActivities.Should().ContainSingle(x => x.OperationName == requestInOperationName && (x.DisplayName == $"POST {Config.EndpointName}" || x.DisplayName == "POST"));
                relevantActivities.Should().ContainSingle(x => x.OperationName == requestInOperationName && x.DisplayName == requestInOperationName);

                relevantActivities.Should().ContainSingle(x => x.OperationName == Config.ActivityName)
                    .Which.TagObjects.Should().ContainKey(customTag).WhoseValue.Should().Be(customValue);

                break;
            case { useAzureMonitor: false, useAspNetCoreInstrumentation: true }:
                relevantActivities.Should().HaveCount(2);

                // For some reason it's inconsistent whether the display name is "POST /" or "POST"
                relevantActivities.Should().ContainSingle(x => x.OperationName == requestInOperationName && (x.DisplayName == $"POST {Config.EndpointName}" || x.DisplayName == "POST"));

                relevantActivities.Should().ContainSingle(x => x.OperationName == Config.ActivityName)
                    .Which.TagObjects.Should().ContainKey(customTag).WhoseValue.Should().Be(customValue);

                break;
            case { useAzureMonitor: false, useAspNetCoreInstrumentation: false }:
                relevantActivities.Should().HaveCount(2);

                relevantActivities.Should().ContainSingle(x => x.OperationName == requestInOperationName)
                    .Which.DisplayName.Should().Be(requestInOperationName);

                relevantActivities.Should().ContainSingle(x => x.OperationName == Config.ActivityName)
                    .Which.TagObjects.Should().ContainKey(customTag).WhoseValue.Should().Be(customValue);

                break;
        }
    }


    private sealed class TestData : IEnumerable<object[]>
    {
        private const int Iterations = 1000;

        public IEnumerator<object[]> GetEnumerator()
        {
            for (var i = 0; i < Iterations; i++)
            {
                yield return [i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}