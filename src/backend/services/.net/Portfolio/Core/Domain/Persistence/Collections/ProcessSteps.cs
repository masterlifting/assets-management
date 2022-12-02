﻿using AM.Services.Common.Contracts.Abstractions.Persistence.Entities.Catalogs;

using Shared.Persistence.Abstractions.Entities;

namespace AM.Services.Portfolio.Core.Domain.Persistence.Collections;

public sealed class ProcessSteps : ProcessStepBase, IPersistentJson
{
    public string JsonVersion { get; init; } = "1.0.0";
}