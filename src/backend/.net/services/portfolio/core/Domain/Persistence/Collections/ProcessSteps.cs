using AM.Services.Common.Abstractions.Persistence.Entities.Catalogs;

using Shared.Persistence.Abstractions.Entities;

namespace AM.Services.Portfolio.Core.Domain.Persistence.Collections;

public sealed class ProcessSteps : ProcessStepBase, IPersistentNoSql
{
    public string JsonVersion { get; init; } = "1.0.0";
}