﻿namespace Shared.Exceptions;

public class SharedSqlException : SharedException
{
    private readonly string _message;
    public SharedSqlException(string initiator, string action, string message) 
        : base(initiator, action) => _message = message;

    public override string Message => base.Message + _message;
}