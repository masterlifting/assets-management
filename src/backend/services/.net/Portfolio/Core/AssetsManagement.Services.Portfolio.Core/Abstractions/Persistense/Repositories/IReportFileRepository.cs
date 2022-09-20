using AM.Services.Portfolio.Core.Domain.Persistense.Entities.States;

using Shared.Persistense.Abstractions.Repositories;

namespace AM.Services.Portfolio.Core.Abstractions.Persistense.Repositories;

public interface IReportFileRepository : IEntityStateRepository<ReportFile>
{
}