using AM.Services.Portfolio.API.Services.Interfaces;
using AM.Services.Portfolio.Core.Domain.Persistence.Collections;
using AM.Services.Portfolio.Core.Domain.Persistence.Entities;

using Microsoft.AspNetCore.Http;

using Shared.Models.Results;
using Shared.Persistence.Abstractions.Repositories;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

using static Shared.Models.Constants.Enums;
using static Shared.Persistence.Abstractions.Constants.Enums;

namespace AM.Services.Portfolio.API.Services
{
    public class ReportApi : IReportApi
    {
        private readonly IMongoDBRepository _mongoRepository;
        private readonly IPostgreSQLRepository _postgreSQLRepository;

        public ReportApi(IMongoDBRepository mongoRepository, IPostgreSQLRepository postgreSQLRepository)
        {
            _mongoRepository = mongoRepository;
            _postgreSQLRepository = postgreSQLRepository;
        }

        public async Task<TryResult<IncomingData[]>> TrySaveFilesAsync(Guid userId, int stepId, IFormFileCollection files)
        {
            #region For TEST
            //var user = await _postgreSQLRepository.FindAsync<User>(userId);

            //if (user is null)
            //    await _postgreSQLRepository.CreateAsync(new User
            //    {
            //        Id = userId,
            //        Name = "Andrey Pestunov"
            //    });
            #endregion

            List<IncomingData> reports = new(files.Count);
            List<string> filesErrors = new(files.Count);

            foreach (var file in files)
            {
                try
                {
                    var payload = new byte[file.Length];
                    await using var stream = file.OpenReadStream();
                    var _ = await stream.ReadAsync(payload.AsMemory(0, (int)file.Length));

                    reports.Add(new()
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId,

                        Payload = payload,
                        PayloadHash = SHA256.HashData(payload),
                        PayloadHashAlgoritm = nameof(SHA256),
                        PayloadSource = file.FileName,
                        PayloadContentType = file.ContentType,

                        ProcessStepId = stepId,
                        ProcessStatusId = (int)ProcessStatuses.Ready
                    });
                }
                catch (Exception exception)
                {
                    filesErrors.Add(exception.Message);
                    continue;
                }
            }

            var savedResult = await _mongoRepository.TryCreateRangeAsync(reports);

            return savedResult.Status != TryResultStatuses.FullSuccess
                ? savedResult
                    : filesErrors.Any()
                    ? new (savedResult.Data!, filesErrors.ToArray())
                : new (savedResult.Data!);
        }
    }
}