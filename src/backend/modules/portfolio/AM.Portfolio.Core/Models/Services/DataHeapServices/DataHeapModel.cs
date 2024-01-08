namespace AM.Portfolio.Core.Models.Services.DataHeapServices;

public sealed record DataHeapModel
{
    public int UserId { get; init; }
    public int StepId { get; init; }
    public byte[] Payload { get; init; } = Array.Empty<byte>();
    public string PayloadSource { get; init; } = string.Empty;
    public string PayloadContentType { get; init; } = string.Empty;
}
