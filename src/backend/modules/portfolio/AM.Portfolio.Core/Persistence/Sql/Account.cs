using AM.Portfolio.Core.Persistence.Entities.Sql.Catalogs;
using Net.Shared.Persistence.Abstractions.Entities;

namespace AM.Portfolio.Core.Persistence.Entities.Sql;

public sealed class Account : IPersistentSql
{
    public int Id { get; set; }
    public string Agreement { get; set; } = null!;

    public User User { get; set; } = null!;
    public int UserId { get; set; }

    public Holder Holder { get; set; } = null!;
    public int HolderId { get; set; }

    public DateTime Created { get; set; }
    public string? Description { get; set; }

    public IEnumerable<Income>? Incomes { get; set; }
    public IEnumerable<Expense>? Expenses { get; set; }
    public IEnumerable<Event>? Events { get; set; }
}
