using Shared.Exceptions.Abstractions;

namespace Shared.Web.Exceptions;

public sealed class SharedWebException : SharedException
{
    private readonly string _message;
    public SharedWebException(string initiator, string action, string message)
        : base(initiator, action) => _message = message;
    public SharedWebException(string initiator, string action, Exception exception)
        : base(initiator, action) => _message = exception.InnerException?.Message ?? exception.Message;

    public override string Message => base.Message + _message;
}