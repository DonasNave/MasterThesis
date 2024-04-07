namespace DTA.Models.Options;

public class RabbitMqOptions
{
    public string HostName { get; set; } = string.Empty;
    public string QueueGroup { get; set; } = string.Empty;
}