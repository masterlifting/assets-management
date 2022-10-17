using Shared.Exceptions.Abstractions;

namespace Shared.Persistense.Exceptions;

public sealed class SharedPersistenseEntityStepException : SharedException
{
    private readonly string _message;
    public SharedPersistenseEntityStepException(string initiator, string action, string message)
        : base(initiator, action) => _message = message;
    public SharedPersistenseEntityStepException(string initiator, string action, Exception exception)
        : base(initiator, action) => _message = exception.InnerException?.Message ?? exception.Message;

    public override string Message => base.Message + _message;
}