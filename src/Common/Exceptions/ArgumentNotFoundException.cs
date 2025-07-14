namespace ascii_cli.Common.Exceptions;

public class ArgumentNotFoundException : Exception
{
    public ArgumentNotFoundException() {}
    public ArgumentNotFoundException(string message) : base(message) {}
    public ArgumentNotFoundException(string message, Exception innerException) : base (message, innerException) {}
}
