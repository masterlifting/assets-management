using Shared.Persistence.Abstractions.Entities;

namespace AM.Services.Portfolio.Core.Domain.Persistence.Entities;

public sealed class User : IPersistent
{
    public Guid Id { get; init; }

    public string Name { get; set; } = null!;

    public DateTime Created { get; init; }
    public string? Info { get; set; }
    
    public IEnumerable<Account>? Accounts { get; set; } = null!;
    public IEnumerable<Event>? Events { get; set; }
    public IEnumerable<Deal>? Deals { get; set; }
}