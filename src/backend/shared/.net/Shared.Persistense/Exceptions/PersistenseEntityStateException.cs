using Shared.Exceptions.Abstractions;

namespace Shared.Persistense.Exceptions;

public class PersistenseEntityStateException : SharedException
{
    private readonly string _message;
    public PersistenseEntityStateException(string initiator, string action, string message)
        : base(initiator, action) => _message = message;
    public PersistenseEntityStateException(string initiator, string action, Exception exception)
        : base(initiator, action) => _message = exception.InnerException?.Message ?? exception.Message;

    public override string Message => base.Message + _message;
}