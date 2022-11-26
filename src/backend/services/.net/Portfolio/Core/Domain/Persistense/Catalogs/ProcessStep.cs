using Shared.Persistense.Abstractions.Entities.Catalogs;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Catalogs;

public sealed class ProcessStep : IProcessableEntityStep
{
    public int Id { get; init; }
    public string Name { get; init; }
    public DateTime Created { get; init; }
    public string? Info { get; set; }
}
