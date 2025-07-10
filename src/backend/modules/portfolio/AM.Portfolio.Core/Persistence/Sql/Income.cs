using AM.Portfolio.Core.Persistence.Entities.Sql.Catalogs;
using Net.Shared.Persistence.Abstractions.Entities;

namespace AM.Portfolio.Core.Persistence.Entities.Sql;

public sealed class Income : IPersistentSql
{
    public long Id { get; set; }

    public Deal Deal { get; set; } = null!;
    public Guid DealId { get; set; }

    public decimal Value { get; set; }
    public DateTime DateTime { get; set; }

    public Derivative Derivative { get; set; } = null!;
    public int DerivativeId { get; set; }

    public Holder Holder { get; set; } = null!;
    public int HolderId { get; set; }

    public Account Account { get; set; } = null!;
    public int AccountId { get; set; }

    public Exchange? Exchange { get; set; }
    public int? ExchangeId { get; set; }

    public DateTime Created { get; set; }
    public string? Description { get; set; }
}
