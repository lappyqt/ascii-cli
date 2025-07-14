namespace ascii_cli.Common.Exceptions;

public class NoRequiredArgumentValueException : Exception
{
    public NoRequiredArgumentValueException() {}
    public NoRequiredArgumentValueException(string message) : base(message) {}
    public NoRequiredArgumentValueException(string message, Exception innerException) : base(message, innerException) {}
}
