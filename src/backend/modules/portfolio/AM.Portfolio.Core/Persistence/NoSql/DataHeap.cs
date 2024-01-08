using AM.Portfolio.Core.Persistence.Entities.Sql;

using MongoDB.Bson;

using Net.Shared.Persistence.Abstractions.Entities;

namespace AM.Portfolio.Core.Persistence.Entities.NoSql;

public sealed class DataHeap : IPersistentNoSql, IPersistentPayload, IPersistentProcess
{
    public ObjectId Id { get; set; }
    public User User { get; set; } = null!;

    public Guid? HostId { get; set; }
    public int StatusId { get; set; }
    public int StepId { get; set; }
    public byte Attempt { get; set; }
    public string? Error { get; set; }

    public byte[] Payload { get; set; } = Array.Empty<byte>();
    public string PayloadSource { get; set; } = null!;
    public byte[] PayloadHash { get; set; } = Array.Empty<byte>();
    public string PayloadHashAlgorithm { get; set; } = null!;
    public string PayloadContentType { get; set; } = null!;

    public DateTime Updated { get; set; } = DateTime.UtcNow;
    public DateTime Created { get; set; } = DateTime.UtcNow;

    public string DocumentVersion { get; set; } = "1.0.0";
    public string? Description { get; set; }
}
