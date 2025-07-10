using Shared.Persistence.Abstractions.Entities;

namespace AM.Services.Portfolio.Core.Domain.Persistence.Collections;

public class IncomingData : IPersistentNoSql, IPersistentPayload, IPersistentProcess
{
    public Guid Id { get; init; }

    public Guid UserId { get; init; }

    public int ProcessStatusId { get; set; }
    public int ProcessStepId { get; set; }
    public byte ProcessAttempt { get; set; }
    public string? Error { get; set; }

    public byte[] Payload { get; init; } = Array.Empty<byte>();
    public string PayloadSource { get; init; } = null!;
    public byte[] PayloadHash { get; init; } = Array.Empty<byte>();
    public string PayloadHashAlgoritm { get; init; } = null!;
    public string PayloadContentType { get; init; } = null!;

    public DateTime Updated { get; set; } = DateTime.UtcNow;
    public DateTime Created { get; init; } = DateTime.UtcNow;

    public string JsonVersion { get; init; } = "1.0.0";
    public string? Description { get; init; }
}