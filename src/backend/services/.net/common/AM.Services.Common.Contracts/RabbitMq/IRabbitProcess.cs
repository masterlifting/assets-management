using System.Collections.Generic;
using System.Threading.Tasks;

namespace AM.Services.Common.Contracts.RabbitMq;

public interface IRabbitProcess
{
    Task ProcessAsync<T>(QueueActions action, T model) where T : class;
    Task ProcessRangeAsync<T>(QueueActions action, IEnumerable<T> models) where T : class;
}