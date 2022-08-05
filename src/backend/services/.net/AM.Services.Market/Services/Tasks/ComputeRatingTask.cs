using AM.Services.Common.Contracts.Background;
using AM.Services.Market.Domain.Entities;
using AM.Services.Market.Services.Entity;

namespace AM.Services.Market.Services.Tasks;

public sealed class ComputeRatingTask : IBackgroundTask
{
    private readonly IServiceScopeFactory scopeFactory;
    public ComputeRatingTask(IServiceScopeFactory scopeFactory) => this.scopeFactory = scopeFactory;

    public async Task StartAsync<T>(T _) where T : BackgroundTaskSettings
    {
        var serviceProvider = scopeFactory.CreateScope().ServiceProvider;
        var ratingService = serviceProvider.GetRequiredService<RatingService>();

        var tasks = await Task.WhenAll(
            ratingService.CompareAsync<Price>(serviceProvider),
            ratingService.CompareAsync<Report>(serviceProvider),
            ratingService.CompareAsync<Coefficient>(serviceProvider),
            ratingService.CompareAsync<Dividend>(serviceProvider));

        if (tasks.Contains(true))
            await ratingService.ComputeAsync();
    }
}