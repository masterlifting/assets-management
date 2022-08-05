using System.Threading.Tasks;

namespace AM.Services.Common.Contracts.RabbitMq;

public interface IRabbitAction
{
    Task GetResultAsync(QueueEntities entity, QueueActions action, string data);
}