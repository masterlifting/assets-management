using AM.Portfolio.Core.Persistence.Entities.Sql;
using AM.Portfolio.Core.Persistence.Entities.Sql.Catalogs;
using AM.Portfolio.Worker.Background.Tasks.StepHandlers;

using Microsoft.Extensions.Options;

using Net.Shared.Background.Abstractions;
using Net.Shared.Background.Core;
using Net.Shared.Models.Settings;
using Net.Shared.Persistence.Abstractions.Entities.Catalogs;
using Net.Shared.Persistence.Abstractions.Repositories.Sql;

namespace AM.Portfolio.Worker.Background.Tasks;

public sealed class EventsBackgroundTask : BackgroundTask<Event>
{
    public const string Name = "HandleEvents";

    private readonly HostSettings _hostSettings;
    private readonly IPersistenceSqlProcessRepository _processRepository;

    public EventsBackgroundTask(
        ILogger logger,
        IOptions<HostSettings> hostOptions,
        IPersistenceSqlProcessRepository processRepository) : base(logger)
    {
        _hostSettings = hostOptions.Value;
        _processRepository = processRepository;
    }

    protected override IBackgroundTaskStep<Event> RegisterStepHandler() =>
        new EventsStepHandler();
    protected override async Task<IPersistentProcessStep[]> GetSteps(CancellationToken cToken) =>
        await _processRepository.GetProcessSteps<ProcessStep>(cToken);
    protected override async Task<Event[]> GetProcessableData(IPersistentProcessStep step, int limit, CancellationToken cToken) =>
        await _processRepository.GetProcessableData<Event>(_hostSettings.Id, step, limit, cToken);
    protected override async Task<Event[]> GetUnprocessedData(IPersistentProcessStep step, int limit, DateTime updateTime, int maxAttempts, CancellationToken cToken) =>
        await _processRepository.GetUnprocessedData<Event>(_hostSettings.Id, step, limit, updateTime, maxAttempts, cToken);
    protected override async Task SaveData(IPersistentProcessStep currentStep, IPersistentProcessStep? nextStep, IEnumerable<Event> data, CancellationToken cToken) =>
        await _processRepository.SetProcessedData(_hostSettings.Id, currentStep, nextStep, data, cToken);
}
