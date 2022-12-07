﻿namespace Shared.MQ.Abstractions.Domain;

public interface IMqConsumerMessage<TPayload> : IMqMessage<TPayload> where TPayload : class
{
    DateTime DateTime { get; init; }
}