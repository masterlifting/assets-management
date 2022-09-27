using AM.Services.Portfolio.Core.Domain.Persistense.Entities;

using Shared.Persistense.Abstractions.Repositories;

namespace AM.Services.Portfolio.Core.Abstractions.Persistense.Repositories
{
    public interface IReportRepository : IRepository<Report>
    {
        Task<(DateOnly dateStart, DateOnly dateEnd)[]> GetReportsDatesAsync(int accountId, DateOnly dateStart, CancellationToken cToken);
    }
}