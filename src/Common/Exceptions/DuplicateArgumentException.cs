namespace ascii_cli.Common.Exceptions;

public class DuplicateArgumentInitException : Exception
{
    public DuplicateArgumentInitException() {}
    public DuplicateArgumentInitException(string message) : base(message) {}
    public DuplicateArgumentInitException(string message, Exception innerException) : base (message, innerException) {}
}
