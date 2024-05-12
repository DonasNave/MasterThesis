namespace DTA.Models.Options;

/// <summary>
/// Represents RabbitMq options
/// </summary>
public class RabbitMqOptions
{
    public string HostName { get; set; } = string.Empty;
    public string QueueGroup { get; set; } = string.Empty;
}