namespace Shared.Exceptions;

public class SharedProcessException : SharedException
{
    private readonly string _message;
    public SharedProcessException(string initiator, string action, string message) 
        : base(initiator, action) => _message = message;
    public SharedProcessException(string initiator, string action, Exception exception) 
        : base(initiator, action) => _message = exception.InnerException?.Message ?? exception.Message;


    public override string Message => base.Message + _message;
}