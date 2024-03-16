namespace DTA.JIT.Workers.Settings;

public class WorkerSettings
{
    public AppWorker[] Workers { get; set; } = [];
    
    public AppWorker? GetWorker(string name)
        => Workers.FirstOrDefault(w => w.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
}

public class AppWorker
{
    public string Name { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
    public string CronSchedule { get; set; } = string.Empty;
}