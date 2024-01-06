using Quartz;

namespace JIT.APP.Workers.Workers;

public class SignalGenerationWorker(HttpClient httpClient) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        // Hit the signal generation endpoint
        var response = await httpClient.GetAsync("http://your-api-server/api/signals/10"); // Adjust the URL and parameters as needed

        if (response.IsSuccessStatusCode)
        {
            // Process the response if needed
        }
        else
        {
            // Handle the error
        }
    }
}
