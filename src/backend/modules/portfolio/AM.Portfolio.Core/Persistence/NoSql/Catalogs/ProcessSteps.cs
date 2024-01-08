using Net.Shared.Persistence.Abstractions.Entities;
using Net.Shared.Persistence.Abstractions.Entities.Catalogs;
using Net.Shared.Persistence.Models.Entities.Catalogs;

namespace AM.Portfolio.Core.Persistence.Entities.NoSql.Catalogs;

public sealed class ProcessSteps : PersistentCatalog, IPersistentNoSql, IPersistentProcessStep
{
    public string DocumentVersion { get; set; } = "1.0.0";
}
