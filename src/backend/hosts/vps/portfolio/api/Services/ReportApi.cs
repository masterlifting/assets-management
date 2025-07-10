using AM.Services.Portfolio.API.Services.Interfaces;
using AM.Services.Portfolio.Core.Abstractions.Persistence;
using AM.Services.Portfolio.Core.Domain.Persistence.Collections;

using Microsoft.AspNetCore.Http;

using Shared.Models.Results;

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;

using static Shared.Persistence.Abstractions.Constants.Enums;

namespace AM.Services.Portfolio.API.Services
{
    public class ReportApi : IReportApi
    {
        private readonly IUnitOfWorkRepository _unitOfWork;

        public ReportApi(IUnitOfWorkRepository unitOfWork)
        {
            _unitOfWork = unitOfWork;
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

            foreach (var file in files)
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

            return await _unitOfWork.IncomingData.Writer.TryCreateManyAsync(reports);
        }
    }
}
