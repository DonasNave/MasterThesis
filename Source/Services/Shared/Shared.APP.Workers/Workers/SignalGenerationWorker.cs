using JIT.APP.Workers.Settings;
using Quartz;

namespace JIT.APP.Workers.Workers;

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
