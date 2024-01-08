using AM.Portfolio.Core.Persistence.Entities.Sql.Catalogs;
using AM.Shared.Abstractions.Persistence.Entities;

using Net.Shared.Persistence.Abstractions.Entities;

namespace AM.Portfolio.Core.Persistence.Entities.Sql;

public sealed class Derivative : IDerivative, IPersistentSql, IPersistentProcess
{
    public int Id { get; init; }

    public string Ticker { get; init; } = null!;
    public string? Code { get; init; }

    public Asset Asset { get; set; } = null!;
    public int AssetId { get; init; }

    public Zone Zone { get; set; } = null!;
    public int ZoneId { get; init; }

    public Balance Balance { get; set; } = null!;

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

    public IEnumerable<Income>? Incomes { get; set; }
    public IEnumerable<Expense>? Expenses { get; set; }
    public IEnumerable<Event>? Events { get; set; }
}
