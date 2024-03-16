using DTA.JIT.Workers.Settings;
using Quartz;

namespace DTA.JIT.Workers.Workers;

public class SignalGenerationWorker(HttpClient httpClient, WorkerSettings settings) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var worker = settings.GetWorker(nameof(SignalGenerationWorker));
        
        if (worker is null) return;
        
        var response = await httpClient.GetAsync(worker.Endpoint); 

        if (response.IsSuccessStatusCode)
        {
        }
        else
        {
        }
    }
}
