using Shared.MessagesQueue.Abstractions.Domain;
using Shared.MessagesQueue.Abstractions.Settings;

using static Shared.MessagesQueue.Constants.Enums.RabbitMq;

namespace Shared.MessagesQueue.Domain.RabbitMq;

public class RabbitMqProducerMessage : IMqProducerMessage<IMqPayload>
{
    public string Id { get; set; } = null!;
    public IMqPayload Payload { get; set; } = null!;
    public IDictionary<string, string> Headers { get; init; } = null!;

    public IMqQueue Queue { get; set; } = null!;
    public ExchangeNames Exchange { get; set; }
    public IMqProducerMessageSettings? Settings { get; set; }

    public string Version { get; set; } = null!;
}