using AM.Portfolio.Core.Persistence.Entities.Sql.Catalogs;
using Net.Shared.Persistence.Abstractions.Entities;

namespace AM.Portfolio.Core.Persistence.Entities.Sql;

public sealed class Event : IPersistentSql, IPersistentProcess
{
    public long Id { get; init; }

    public decimal Value { get; set; }
    public DateTime DateTime { get; set; }

    public EventType Type { get; set; } = null!;
    public int TypeId { get; set; }

    public Derivative Derivative { get; set; } = null!;
    public int DerivativeId { get; set; }

    public User User { get; set; } = null!;
    public int UserId { get; set; }

    public Account Account { get; set; } = null!;
    public int AccountId { get; set; }

    public Holder Holder { get; set; } = null!;
    public int HolderId { get; set; }

    public Exchange? Exchange { get; set; }
    public int? ExchangeId { get; set; }

    public Guid? HostId { get; set; }
    public ProcessStep Step { get; set; } = null!;
    public int StepId { get; set; }
    public ProcessStatus Status { get; set; } = null!;
    public int StatusId { get; set; }
    public byte Attempt { get; set; }
    public string? Error { get; set; }

    public DateTime Updated { get; set; }
    public DateTime Created { get; set; }

    public string? Description { get; set; }
}
