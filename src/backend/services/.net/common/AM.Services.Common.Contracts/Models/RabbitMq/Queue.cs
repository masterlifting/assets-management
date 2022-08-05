using System;
using AM.Services.Common.Contracts.RabbitMq;

namespace AM.Services.Common.Contracts.Models.RabbitMq;

public class Queue
{
    public Queue(QueueNames name, bool withConfirm = false)
    {
        NameString = name.ToString();
        NameEnum = name;
        WithConfirm = withConfirm;
    }

    public string NameString { get; }
    public QueueNames NameEnum { get; }
    public bool WithConfirm { get; }
    public QueueEntity[] Entities { get; init; } = Array.Empty<QueueEntity>();
}