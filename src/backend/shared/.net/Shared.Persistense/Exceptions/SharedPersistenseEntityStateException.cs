using Shared.Exceptions.Abstractions;
using Shared.Exceptions.Models;

namespace Shared.Persistense.Exceptions;

public sealed class SharedPersistenseEntityStateException : SharedException
{
    public SharedPersistenseEntityStateException(string initiator, string action, ExceptionDescription description) : base(initiator, action, description) { }
}