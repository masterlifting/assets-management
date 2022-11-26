using AM.Services.Portfolio.Core.Domain.Persistense.Entities;

using Shared.Persistense.Abstractions.Entities.Catalogs;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Catalogs;

public sealed class Provider : EntityCatalog
{
    public IEnumerable<Deal>? Deals { get; set; }
    public IEnumerable<Event>? Events { get; set; }
    public IEnumerable<Account>? Accounts { get; set; }
}