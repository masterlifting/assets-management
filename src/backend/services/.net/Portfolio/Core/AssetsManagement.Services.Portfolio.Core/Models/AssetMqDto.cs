using Shared.Infrastructure.Mq.Interfaces;

namespace AM.Services.Portfolio.Core.Models;

public class AssetMqDto : IMqProducerMessage<string>
{
    public string Payload { get; set; } = null!;
    public IDictionary<string, string> Headers { get; init; } = null!;
    public IMqQueue Queue { get; set; } = null!;
    public IMqProducerMessageSettings? Settings { get; set; }
    public string Version { get; set; } = "0.0.1";
}