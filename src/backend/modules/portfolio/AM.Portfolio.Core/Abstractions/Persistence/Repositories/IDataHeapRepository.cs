using AM.Portfolio.Core.Models.Services.DataHeapServices;
using AM.Portfolio.Core.Persistence.Entities.NoSql;

using Net.Shared.Models.Domain;

namespace AM.Portfolio.Core.Abstractions.Persistence.Repositories;

public interface IDataHeapRepository
{
    Task<Result<DataHeap>> TryCreate(DataHeapModel model, CancellationToken cToken);
}
