namespace ascii_cli.Common.Exceptions;

public class DuplicateArgumentParseException : Exception
{
    public DuplicateArgumentParseException() {}
    public DuplicateArgumentParseException(string message) : base(message) {}
    public DuplicateArgumentParseException(string message, Exception innerException) : base (message, innerException) {}
}
