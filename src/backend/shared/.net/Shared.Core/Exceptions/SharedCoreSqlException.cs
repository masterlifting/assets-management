﻿namespace Shared.Core.Exceptions;

public class SharedCoreSqlException : SharedCoreException
{
    private readonly string _message;
    public SharedCoreSqlException(string initiator, string action, string message) 
        : base(initiator, action) => _message = message;

    public override string Message => base.Message + _message;
}