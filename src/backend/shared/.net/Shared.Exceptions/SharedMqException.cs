namespace Shared.Exceptions;

public class SharedMqException : SharedException
{
    private readonly string _message;
    public SharedMqException(string initiator, string action, string message) 
        : base(initiator, action) => _message = message;

    public override string Message => base.Message + _message;
}