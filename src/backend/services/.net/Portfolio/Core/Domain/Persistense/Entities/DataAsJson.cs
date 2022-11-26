using Shared.Persistense.Abstractions.Entities;

using System.Text.Json;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Entities;

public class DataAsJson : IEntityDataAsJson
{
    public JsonDocument Json { get; init; }
    public string JsonVersion { get; init; }
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
