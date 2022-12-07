using Shared.Exceptions.Abstractions;
using Shared.Exceptions.Models;

namespace Shared.MQ.Exceptions;

public sealed class SharedMQException : SharedException
{
    public SharedMQException(string initiator, string action, ExceptionDescription description) : base(initiator, action, description) { }
}