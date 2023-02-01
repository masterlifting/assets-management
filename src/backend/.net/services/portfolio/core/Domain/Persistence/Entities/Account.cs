using AM.Services.Portfolio.Core.Domain.Persistence.Entities.Catalogs;

using Shared.Persistence.Abstractions.Entities;

namespace AM.Services.Portfolio.Core.Domain.Persistence.Entities;

public sealed class Account : IPersistentSql
{
    public int Id { get; set; }
    public string Name { get; init; } = null!;

    public string? Description { get; init; }

    public DateTime Created { get; init; }

    public User User { get; init; } = null!;
    public Guid UserId { get; init; }

    public Provider Provider { get; init; } = null!;
    public int ProviderId { get; init; }

    public IEnumerable<Deal>? Deals { get; set; }
    public IEnumerable<Event>? Events { get; set; }
}