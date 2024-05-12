using Microsoft.AspNetCore.Http;

namespace DTA.Extensions.Telemetry;

/// <summary>
/// Extensions for the HTTP request
/// </summary>
public static class HttpRequestExtensions
{
    /// <summary>
    /// Get the test tags from the request
    /// </summary>
    /// <param name="request">The HTTP request</param>
    public static KeyValuePair<string, object?>[] GetTestTags(this HttpRequest request)
    {
        var testTags = new List<KeyValuePair<string, object?>>();
        var testId = request.Headers["test_id"].FirstOrDefault();

        if (!string.IsNullOrEmpty(testId))
            testTags.Add(new KeyValuePair<string, object?>("test_id", testId));

        return testTags.ToArray();
    }
}