using Shared.Persistense.Abstractions.Entities;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Entities;

public class DataAsBytes : IEntityDataAsByte
{
    public byte[] Payload { get; init; }
    public string PayloadSource { get; init; }
    public string PayloadContentType { get; init; }
    public Guid UserId { get; init; }
    public byte[] SHA256Hash { get; init; }
    public Guid Id { get; init; }
    public int ProcessStatusId { get; set; }
    public int ProcessStepId { get; set; }
    public byte ProcessAttempt { get; set; }
    public DateTime Updated { get; set; }
    public DateTime Created { get; init; }
    public string? Info { get; set; }
}
