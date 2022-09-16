namespace Shared.Exceptions;

public class SharedCastException : SharedException
{
    private readonly string _message;
    public SharedCastException(string initiator, string action, string message) 
        : base(initiator, action) => _message = message;

    public override string Message => base.Message + _message;
}