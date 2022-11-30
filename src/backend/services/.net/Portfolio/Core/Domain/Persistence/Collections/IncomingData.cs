using Shared.Persistence.Abstractions.Entities;

namespace AM.Services.Portfolio.Core.Domain.Persistence.Collections;

public class IncomingData : IPersistentPayload, IPersistentProcess, IPersistentJson
{
    public Guid Id { get; init; }

    public Guid UserId { get; init; }

    public int ProcessStatusId { get; set; }
    public int ProcessStepId { get; set; }
    public byte ProcessAttempt { get; set; }
    
    public byte[] Payload { get; init; } = Array.Empty<byte>();
    public string PayloadSource { get; init; } = null!;
    public byte[] PayloadHash { get; init; } = Array.Empty<byte>();
    public string PayloadHashAlgoritm { get; init; } = null!;
    public string PayloadContentType { get; init; } = null!;

    public DateTime Updated { get; set; }
    public DateTime Created { get; init; }
    public string? Info { get; set; }

    public string JsonVersion { get; init; } = "1.0.0";
}