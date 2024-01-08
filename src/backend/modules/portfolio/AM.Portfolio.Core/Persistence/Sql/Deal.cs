using AM.Portfolio.Core.Persistence.Entities.Sql.Catalogs;
using Net.Shared.Persistence.Abstractions.Entities;

namespace AM.Portfolio.Core.Persistence.Entities.Sql;

public sealed class Deal : IPersistentSql, IPersistentProcess
{
    public Guid Id { get; init; }

    public Income Income { get; set; } = null!;
    public Expense Expense { get; set; } = null!;

    public DateTime DateTime { get; set; }

    public User User { get; set; } = null!;
    public int UserId { get; set; }

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
