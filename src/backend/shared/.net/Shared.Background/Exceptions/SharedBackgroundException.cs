using Shared.Exceptions.Abstractions;

namespace Shared.Background.Exceptions;
public sealed class SharedBackgroundException : SharedException
{
    private readonly string _message;
    public SharedBackgroundException(string initiator, string action, string message)
        : base(initiator, action) => _message = message;
    public SharedBackgroundException(string initiator, string action, Exception exception)
        : base(initiator, action) => _message = exception.InnerException?.Message ?? exception.Message;

    public override string Message => base.Message + _message;
}