using Shared.Exceptions.Abstractions;
using Shared.Exceptions.Models;

namespace Shared.MessagesQueue.Exceptions;

public sealed class SharedMessagesQueueException : SharedException
{
    public SharedMessagesQueueException(string initiator, string action, ExceptionDescription description) : base(initiator, action, description) { }
}