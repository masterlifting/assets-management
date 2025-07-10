using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using AM.Portfolio.Api.Abstractions.Services;
using AM.Portfolio.Core.Abstractions.Persistence.Repositories;
using AM.Portfolio.Core.Models.Services.DataHeapServices;
using AM.Portfolio.Core.Persistence.Entities.NoSql;

using Microsoft.AspNetCore.Http;

using Net.Shared.Models.Domain;

namespace AM.Portfolio.Api.Services;

public sealed class ReportsService : IReportsService
{
    private readonly IDataHeapRepository _dataHeapRepository;
    public ReportsService(IDataHeapRepository dataHeapRepository) => _dataHeapRepository = dataHeapRepository;

    public async Task<Result<DataHeap>> Post(int userId, int stepId, IFormFileCollection files, CancellationToken cToken)
    {
        var result = new List<DataHeap>(files.Count);
        var errors = new List<string>(files.Count);

        foreach (var file in files)
        {
            var payload = new byte[file.Length];
            await using var stream = file.OpenReadStream();
            var _ = await stream.ReadAsync(payload.AsMemory(0, (int)file.Length), cToken);

            var model = new DataHeapModel
            {
                UserId = userId,
                StepId = stepId,
                Payload = payload,
                PayloadSource = file.FileName,
                PayloadContentType = file.ContentType
            };

            var createResult = await _dataHeapRepository.TryCreate(model, cToken);

            errors.AddRange(createResult.Errors);
            result.AddRange(createResult.Data);
        }

        return new(result, errors);
    }
}
