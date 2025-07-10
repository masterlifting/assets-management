using Shared.Exceptions.Abstractions;
using Shared.Exceptions.Models;

namespace Shared.Queue.Exceptions;

public sealed class SharedQueueException : SharedException
{
    public SharedQueueException(string initiator, string action, ExceptionDescription description) : base(initiator, action, description) { }
}
