using Shared.Persistense.Abstractions.Entities;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Collections;

public class IncomingData : IPersistensablePayload, IPersistensableProcess
{
    public Guid Id { get; init; }
    
    public int ProcessStatusId { get; set; }
    public int ProcessStepId { get; set; }
    
    public byte ProcessAttempt { get; set; }
    public DateTime Updated { get; set; }
    public DateTime Created { get; init; }
    public string? Info { get; set; }

    public byte[] Payload { get; init; } = Array.Empty<byte>();
    public byte[] PayloadHash { get; init; } = Array.Empty<byte>();
    public string PayloadSource { get; init; } = null!;
    public string PayloadContentType { get; init; } = null!;
    public string PayloadHashAlgoritm { get; init; } = null!;
}