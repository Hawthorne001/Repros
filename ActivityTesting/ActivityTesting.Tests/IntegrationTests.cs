using System.Collections;
using System.Diagnostics;
using ActivityTesting.API;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ActivityTesting.Tests;

public sealed class IntegrationTests
{
    // newing it up for simplicity's sake
    private static readonly IConfiguration Configuration =
        new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

    private static List<Activity> GetRelevantActivities(IEnumerable<Activity> activities, Activity activity)
    {
        return [..activities.Where(x => x.TraceId == activity.TraceId).Except([activity])];
    }

    [Theory]
    [ClassData(typeof(TestData))]
    public async Task MultiTest(int something)
    {
        // Arrange
        List<Activity> activities = [];

        var factory = new WebApplicationFactory<ApiAssemblyMarker>();
        var inProcessActivitySource = factory.Services.GetRequiredService<ActivitySource>();
        
        var listener = new ActivityListener();
        listener.Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData;
        listener.ShouldListenTo = source => source == inProcessActivitySource;
        listener.ActivityStarted = a =>
        {
            lock (activities)
            {
                activities.Add(a);
            }
        };
        ActivitySource.AddActivityListener(listener);
        
        using var activity = inProcessActivitySource.StartActivity("my-activity")!;
        var activityId = activity.Id;
        var httpContent = new StringContent("some content");
        httpContent.Headers.TryAddWithoutValidation("traceparent", activityId);
        using var client = factory.CreateClient();
        var uri = new Uri(client.BaseAddress!, Config.EndpointName); 

        // Act
        var response = await client.PostAsync(uri, httpContent);

        // Assert
        var relevantActivities = GetRelevantActivities(activities, activity);

        const string requestInOperationName = "Microsoft.AspNetCore.Hosting.HttpRequestIn";
        const string customTag = "my-tag";
        const string customValue = "my-value";

        relevantActivities.Should().HaveCount(2);

        relevantActivities.Should().ContainSingle(x => x.OperationName == requestInOperationName)
            .Which.DisplayName.Should().BeOneOf("POST", $"POST {Config.EndpointName}");

        relevantActivities.Should().ContainSingle(x => x.OperationName == Config.ActivityName)
            .Which.TagObjects.Should().ContainKey(customTag).WhoseValue.Should().Be(customValue);
    }


    private sealed class TestData : IEnumerable<object[]>
    {
        private const int Iterations = 250;

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
