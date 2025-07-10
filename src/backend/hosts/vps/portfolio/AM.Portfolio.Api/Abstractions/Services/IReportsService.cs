using System.Threading;
using System.Threading.Tasks;

using AM.Portfolio.Core.Persistence.Entities.NoSql;

using Microsoft.AspNetCore.Http;

using Net.Shared.Models.Domain;

namespace AM.Portfolio.Api.Abstractions.Services;

public interface IReportsService
{
    Task<Result<DataHeap>> Post(int userId, int stepId, IFormFileCollection files, CancellationToken cToken);
}
