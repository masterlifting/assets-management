using AM.Portfolio.Core.Abstractions.Services.DataHeapServices.Raiffeisen.Serbia.Transactions;
using AM.Portfolio.Core.Persistence.Entities.NoSql;

using Net.Shared.Persistence.Abstractions.Contexts;
using Net.Shared.Queues.Abstractions.Core.WorkQueue;

using static Net.Shared.Persistence.Models.Constants.Enums;

namespace AM.Portfolio.Core.Services.DataHeapServices.Raiffeisen.Serbia.Transactions;

public sealed class RaiffeisenSrbTransactionsHandler : IRaiffeisenSrbTransactionsHandler
{
    private readonly IRaiffeisenSrbTransactionsParser _parser;
    private readonly IRaiffeisenSrbTransactionsMapper _mapper;
    private readonly IWorkQueue _queue;
    private readonly IPersistenceSqlContext _persistence;

    public RaiffeisenSrbTransactionsHandler(
        IRaiffeisenSrbTransactionsParser parser
        , IRaiffeisenSrbTransactionsMapper mapper
        , IWorkQueue queue
        , IPersistenceSqlContext context)
    {
        _parser = parser;
        _mapper = mapper;
        _queue = queue;
        _persistence = context;
    }
    public async Task Handle(IEnumerable<DataHeap> data, CancellationToken cToken)
    {
        var handlers = data.Select(async x =>
        {
            try
            {
                var parsingResult = _parser.Parse(x.PayloadSource, x.Payload);

                await _queue.Process(() => _mapper.Map(parsingResult, cToken), cToken);
            }
            catch (Exception exception)
            {
                x.StatusId = (int)ProcessStatuses.Error;
                x.Error = exception.Message;
            }
        });

        await Task.WhenAll(handlers);

        try
        {
            await _persistence.StartTransaction(cToken);
            await _persistence.CreateMany(_mapper.Deals, cToken);
            await _persistence.CreateMany(_mapper.Events, cToken);
            await _persistence.CommitTransaction(cToken);
        }
        catch
        {
            await _persistence.RollbackTransaction(cToken);
            throw;
        }
    }
}
