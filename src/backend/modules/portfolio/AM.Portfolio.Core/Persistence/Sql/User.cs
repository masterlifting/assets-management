using Net.Shared.Persistence.Abstractions.Entities;

namespace AM.Portfolio.Core.Persistence.Entities.Sql;

public sealed class User : IPersistentSql
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public DateTime Created { get; set; }
    public string? Description { get; set; }

    public IEnumerable<Account>? Accounts { get; set; }
    public IEnumerable<Event>? Events { get; set; }
    public IEnumerable<Deal>? Deals { get; set; }
}
