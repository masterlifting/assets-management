using AM.Portfolio.Core.Persistence.Entities.NoSql;

namespace AM.Portfolio.Core.Abstractions.Services.DataHeapServices;

public interface IDataHeapHandler
{
    Task Handle(IEnumerable<DataHeap> data, CancellationToken cToken);
}
