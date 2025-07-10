using AM.Portfolio.Core.Abstractions.Services.DataHeapServices.Bcs.Companies;
using AM.Portfolio.Core.Persistence.Entities.NoSql;

using Net.Shared.Persistence.Abstractions.Contexts;
using Net.Shared.Queues.Abstractions.Core.WorkQueue;

using static Net.Shared.Persistence.Models.Constants.Enums;

namespace AM.Portfolio.Core.Services.DataHeapServices.Bcs.Companies;

public sealed class BcsCompaniesHandler : IBcsCompaniesHandler
{
    private readonly IWorkQueue _queue;
    private readonly IBcsCompaniesParser _parser;
    private readonly IBcsCompaniesMapper _mapper;
    private readonly IPersistenceSqlContext _persistence;

    public BcsCompaniesHandler(IBcsCompaniesParser parser, IBcsCompaniesMapper mapper, IWorkQueue queue, IPersistenceSqlContext context)
    {
        _parser = parser;
        _mapper = mapper;
        _queue = queue;
        _persistence = context;
    }
    public Task Handle(IEnumerable<DataHeap> data, CancellationToken cToken)
    {
        var handlers = data.Select(async x =>
        {
            try
            {
                var parsingResult = _parser.Parse(x.PayloadSource, x.Payload);

                await _queue.Process( async () =>
                {
                    try
                    {
                        await _persistence.StartTransaction(cToken);
                        await _mapper.Map(parsingResult, cToken);
                        await _persistence.CommitTransaction(cToken);
                    }
                    catch
                    {
                        await _persistence.RollbackTransaction(cToken);
                        throw;
                    }
                }
                , cToken);
            }
            catch (Exception exception)
            {
                x.StatusId = (int)ProcessStatuses.Error;
                x.Error = exception.Message;
            }
        });

        return Task.WhenAll(handlers);
    }
}
