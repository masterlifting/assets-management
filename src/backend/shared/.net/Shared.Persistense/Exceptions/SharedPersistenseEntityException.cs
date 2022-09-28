using Shared.Exceptions.Abstractions;

namespace Shared.Persistense.Exceptions;

public sealed class SharedPersistenseEntityException : SharedException
{
    private readonly string _message;
    public SharedPersistenseEntityException(string initiator, string action, string message)
        : base(initiator, action) => _message = message;
    public SharedPersistenseEntityException(string initiator, string action, Exception exception)
        : base(initiator, action) => _message = exception.InnerException?.Message ?? exception.Message;

    public override string Message => base.Message + _message;
}