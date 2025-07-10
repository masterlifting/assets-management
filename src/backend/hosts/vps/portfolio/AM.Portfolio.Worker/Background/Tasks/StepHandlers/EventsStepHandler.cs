using AM.Portfolio.Core.Persistence.Entities.Sql;

using Net.Shared.Background.Abstractions;
using Net.Shared.Models.Domain;
using Net.Shared.Persistence.Abstractions.Entities.Catalogs;

using static AM.Portfolio.Core.Constants.Enums;

namespace AM.Portfolio.Worker.Background.Tasks.StepHandlers;

public sealed class EventsStepHandler : IBackgroundTaskStep<Event>
{
    public EventsStepHandler()
    {
    }

    public async Task<Result<Event>> Handle(IPersistentProcessStep step, IEnumerable<Event> data, CancellationToken cToken)
    {
        switch (step.Id)
        {
            case (int)ProcessSteps.CalculateSplitting:
                {
                    return new(data);
                }
            case (int)ProcessSteps.CalculateBalance:
                {
                    return new(data);
                }
            default:
                throw new NotImplementedException($"The step '{step.Name}' of the task '{EventsBackgroundTask.Name}' was not recognized.");
        }
    }
}
