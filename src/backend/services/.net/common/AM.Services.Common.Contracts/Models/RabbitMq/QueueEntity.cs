using System;
using AM.Services.Common.Contracts.RabbitMq;

namespace AM.Services.Common.Contracts.Models.RabbitMq;

public class QueueEntity
{
    public QueueEntity(QueueEntities entity)
    {
        NameString = entity.ToString();
        NameEnum = entity;
    }
    public string NameString { get; }
    public QueueEntities NameEnum { get; }
    public QueueActions[] Actions { get; init; } = Array.Empty<QueueActions>();
}