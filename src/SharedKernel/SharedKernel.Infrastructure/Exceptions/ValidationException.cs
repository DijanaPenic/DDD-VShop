using System;

namespace VShop.SharedKernel.Infrastructure.Exceptions;

public abstract class ValidationException : Exception
{
    protected ValidationException(string message, string parameter = null) 
        : base(string.IsNullOrEmpty(parameter) ? message : $"{parameter}: {message}") { }
}