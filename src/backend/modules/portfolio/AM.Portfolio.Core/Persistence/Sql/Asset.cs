using AM.Portfolio.Core.Persistence.Entities.Sql.Catalogs;
using AM.Shared.Abstractions.Persistence.Entities;

using Net.Shared.Persistence.Abstractions.Entities;

namespace AM.Portfolio.Core.Persistence.Entities.Sql;

public sealed class Asset : IAsset, IPersistentSql, IPersistentProcess
{
    public int Id { get; init; }

    public string Name { get; init; } = null!;

    public AssetType Type { get; init; } = null!;
    public int TypeId { get; init; }
    
    public string? Label { get; init; }

    public Guid? HostId { get; set; }
    public ProcessStep Step { get; set; } = null!;
    public int StepId { get; set; }
    public ProcessStatus Status { get; set; } = null!;
    public int StatusId { get; set; }
    public byte Attempt { get; set; }
    public string? Error { get; set; }

    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }

    public string? Description { get; set; }
}
