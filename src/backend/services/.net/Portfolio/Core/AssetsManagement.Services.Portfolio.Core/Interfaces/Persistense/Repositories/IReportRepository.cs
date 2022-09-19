using AM.Services.Portfolio.Core.Domain.Persistense.Entities.States;
using AM.Services.Portfolio.Core.Domain.Persistense.Models.ValueObjects;
using Shared.Infrastructure.Persistense.Abstractions.Repositories;

namespace AM.Services.Portfolio.Core.Interfaces.Persistense.Repositories;

public interface IReportRepository : IEntityStateRepository<Report>
{
    Task<(DateOnly dateStart, DateOnly dateEnd)[]> GetReportDatesAsync(int accountId, ProviderId providerId, DateOnly dateStart, CancellationToken cToken);
}