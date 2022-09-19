﻿using Shared.Exceptions.Abstractions;

namespace Shared.MessagesQueue.Exceptions;
public sealed class SharedMessagesQueueException : SharedException
{
    private readonly string _message;
    public SharedMessagesQueueException(string initiator, string action, string message)
        : base(initiator, action) => _message = message;
    public SharedMessagesQueueException(string initiator, string action, Exception exception)
        : base(initiator, action) => _message = exception.InnerException?.Message ?? exception.Message;

    public override string Message => base.Message + _message;
}