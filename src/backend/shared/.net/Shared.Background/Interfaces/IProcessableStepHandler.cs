﻿using Shared.Persistense.Abstractions.Entities;

namespace Shared.Background.Interfaces;

public interface IProcessableStepHandler<T> where T : class, IEntityProcessable
{
    Task HandleStepAsync(IEnumerable<T> entities, CancellationToken cToken);
    Task<IReadOnlyCollection<T>> HandleStepAsync(CancellationToken cToken);
}