using Shared.Queue.Abstractions.Domain;
using Shared.Queue.Abstractions.Settings;

using static Shared.Queue.Constants.Enums.RabbitMq;

namespace Shared.Queue.Domain.RabbitMq;

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