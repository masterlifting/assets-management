using System.Security.Cryptography;

using AM.Portfolio.Core.Abstractions.Persistence.Repositories;
using AM.Portfolio.Core.Exceptions;
using AM.Portfolio.Core.Models.Services.DataHeapServices;
using AM.Portfolio.Core.Persistence.Entities.NoSql;
using AM.Portfolio.Core.Persistence.Entities.NoSql.Catalogs;
using AM.Portfolio.Core.Persistence.Entities.Sql;
using AM.Portfolio.Core.Persistence.Entities.Sql.Catalogs;

using Net.Shared.Models.Domain;
using Net.Shared.Persistence.Abstractions.Repositories.NoSql;
using Net.Shared.Persistence.Abstractions.Repositories.Sql;

using static Net.Shared.Persistence.Models.Constants.Enums;

namespace AM.Portfolio.Infrastructure.Persistence.Repositories;

public sealed class DataHeapRepository : IDataHeapRepository
{
    private readonly IPersistenceNoSqlWriterRepository _noSqlWriterRepository;
    private readonly IPersistenceNoSqlReaderRepository _noSqlReaderRepository;
    private readonly IPersistenceSqlReaderRepository _sqlReaderRepository;

    public DataHeapRepository(
        IPersistenceNoSqlWriterRepository noSqlWriterRepository,
        IPersistenceNoSqlReaderRepository noSqlReaderRepository,
        IPersistenceSqlReaderRepository sqlReaderRepository)
    {
        _sqlReaderRepository = sqlReaderRepository;
        _noSqlWriterRepository = noSqlWriterRepository;
        _noSqlReaderRepository = noSqlReaderRepository;
    }

    public async Task<Result<DataHeap>> TryCreate(DataHeapModel model, CancellationToken cToken)
    {
        var user = await _sqlReaderRepository.FindById<User>(model.UserId, cToken);

        if (user is null)
            return new(new PortfolioCoreException($"User with id '{model.UserId}' was not found."));

        var step = await _noSqlReaderRepository.GetCatalogById<ProcessSteps>(model.StepId, cToken);

        var status = await _sqlReaderRepository.GetCatalogByEnum<ProcessStatus, ProcessStatuses>(ProcessStatuses.Ready, cToken);

        var report = new DataHeap()
        {
            User = user,

            Payload = model.Payload,
            PayloadHash = SHA256.HashData(model.Payload),
            PayloadHashAlgorithm = nameof(SHA256),
            PayloadSource = model.PayloadSource,
            PayloadContentType = model.PayloadContentType,

            StepId = step.Id,
            StatusId = status.Id
        };

        return await _noSqlWriterRepository.TryCreateOne(report, cToken);
    }
}
