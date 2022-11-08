using Shared.Exceptions.Abstractions;
using Shared.Exceptions.Models;

namespace Shared.Persistense.Exceptions;

public sealed class SharedPersistenseEntityException : SharedException
{
    public SharedPersistenseEntityException(string initiator, string action, ExceptionDescription description) : base(initiator, action, description) { }
}