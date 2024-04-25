using Microsoft.AspNetCore.Http;

namespace DTA.Extensions.Telemetry;

public static class HttpRequestExtensions
{
    public static KeyValuePair<string, object?>[] GetTestTags(this HttpRequest request)
    {
        var testTags = new List<KeyValuePair<string, object?>>();
        var testId = request.Headers["test_id"].FirstOrDefault();
        
        if (!string.IsNullOrEmpty(testId))
            testTags.Add(new KeyValuePair<string, object?>("test_id", testId));
        
        return testTags.ToArray();
    }
}