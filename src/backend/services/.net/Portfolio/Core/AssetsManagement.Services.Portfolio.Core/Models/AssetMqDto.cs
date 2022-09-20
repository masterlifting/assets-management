using Shared.MessagesQueue.Abstractions.Domain;
using Shared.MessagesQueue.Abstractions.Settings;

namespace AM.Services.Portfolio.Core.Models;

public sealed class AssetMqDto : IMqProducerMessage<string>
{
    public string Payload { get; set; } = null!;
    public IDictionary<string, string> Headers { get; init; } = null!;
    public IMqQueue Queue { get; set; } = null!;
    public IMqProducerMessageSettings? Settings { get; set; }
    public string Version { get; set; } = "0.0.1";
}