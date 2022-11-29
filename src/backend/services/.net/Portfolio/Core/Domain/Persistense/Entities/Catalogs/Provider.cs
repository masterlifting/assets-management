using Shared.Persistense.Abstractions.Entities.Catalogs;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Entities.Catalogs;

public sealed class Provider : PersistensableCatalog
{
    public IEnumerable<Deal>? Deals { get; set; }
    public IEnumerable<Event>? Events { get; set; }
    public IEnumerable<Account>? Accounts { get; set; }
}