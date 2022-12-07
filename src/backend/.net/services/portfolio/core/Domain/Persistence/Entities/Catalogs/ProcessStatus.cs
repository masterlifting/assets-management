using AM.Services.Common.Abstractions.Persistence.Entities.Catalogs;

namespace AM.Services.Portfolio.Core.Domain.Persistence.Entities.Catalogs;

public sealed class ProcessStatus : ProcessStatusBase
{
    public IEnumerable<Asset>? Assets { get; set; }
    public IEnumerable<Derivative>? Derivatives { get; set; }
    public IEnumerable<Deal>? Deals { get; set; }
    public IEnumerable<Event>? Events { get; set; }
}