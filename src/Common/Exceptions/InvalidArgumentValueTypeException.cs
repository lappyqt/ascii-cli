namespace ascii_cli.Common.Exceptions;

public class InvalidArgumentValueTypeException : Exception
{
    public InvalidArgumentValueTypeException() {}
    public InvalidArgumentValueTypeException(string message) : base(message) {}
    public InvalidArgumentValueTypeException(string message, Exception innerException) : base(message, innerException) {}
}
