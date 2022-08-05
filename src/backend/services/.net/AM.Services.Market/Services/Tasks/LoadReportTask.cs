using AM.Services.Common.Contracts.Background;
using AM.Services.Market.Domain.DataAccess;
using AM.Services.Market.Domain.Entities;
using AM.Services.Market.Domain.Entities.ManyToMany;
using AM.Services.Market.Services.Data;
using AM.Services.Market.Settings.Sections;

namespace AM.Services.Market.Services.Tasks;

public sealed class LoadReportTask : IBackgroundTask
{
    private readonly IServiceScopeFactory scopeFactory;
    public LoadReportTask(IServiceScopeFactory scopeFactory) => this.scopeFactory = scopeFactory;

    public async Task StartAsync<T>(T settings) where T : BackgroundTaskSettings
    {
        var serviceProvider = scopeFactory.CreateAsyncScope().ServiceProvider;

        var loader = serviceProvider.GetRequiredService<DataLoader<Report>>();
        var companySourcesRepo = serviceProvider.GetRequiredService<Repository<CompanySource>>();

        var _settings = settings as LoadTaskSettings ?? throw new ApplicationException("Report settings not found");
        var sourceIds = _settings.Sources.Select(x => (byte)x).ToArray();

        var companySources = await companySourcesRepo.GetSampleAsync(x => sourceIds.Contains(x.SourceId));

        await loader.LoadAsync(companySources);
    }
}