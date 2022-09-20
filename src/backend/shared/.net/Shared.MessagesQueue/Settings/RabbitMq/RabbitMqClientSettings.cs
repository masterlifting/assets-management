namespace Shared.MessagesQueue.Settings.RabbitMq;

public class RabbitMqClientSettings
{
    public string Host { get; set; } = null!;
    public string User { get; set; } = null!;
    public string Password { get; set; } = null!;
    public RabbitMqModelBuilderSettings[] ModelBuilders { get; set; } = Array.Empty<RabbitMqModelBuilderSettings>();
}