using Shared.Background.Exceptions;
using Shared.Background.Interfaces;
using Shared.Persistense.Abstractions.Entities;
using Shared.Persistense.Abstractions.Entities.Catalogs;

namespace Shared.Background.Core.Handlers;

public sealed class BackgroundTaskStepHandler<T> where T : class, IPersistensableProcess
{
    private readonly Dictionary<int, IProcessableStepHandler<T>> _handlers;
    public BackgroundTaskStepHandler(Dictionary<int, IProcessableStepHandler<T>> handlers) => _handlers = handlers;

    public Task HandleAsync(IProcessableStep step, IEnumerable<T> data, CancellationToken cToken) => _handlers.ContainsKey(step.Id)
        ? _handlers[step.Id].HandleStepAsync(data, cToken)
        : throw new SharedBackgroundException(typeof(T).Name + "Handler", nameof(HandleAsync), new($"Step: '{step.Name}' not implemented'"));
    public Task<IReadOnlyCollection<T>> HandleAsync(IProcessableStep step, CancellationToken cToken) => _handlers.ContainsKey(step.Id)
        ? _handlers[step.Id].HandleStepAsync(cToken)
        : throw new SharedBackgroundException(typeof(T).Name + "Handler", nameof(HandleAsync), new($"Step: '{step.Name}' not implemented'"));
}