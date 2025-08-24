namespace ascii_cli.Common.Exceptions;

public class NoSuitableOptionFoundException : Exception
{
    public NoSuitableOptionFoundException() {}
    public NoSuitableOptionFoundException(string message) : base(message) {}
    public NoSuitableOptionFoundException(string message, Exception innerException) : base (message, innerException) {}
}
