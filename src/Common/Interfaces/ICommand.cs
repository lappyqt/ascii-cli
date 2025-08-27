namespace ascii_cli.Common.Interfaces;

public interface ICommand
{
    public string Name { get; }
    public string? Description { get; }

    Argument[] GetArguments();
    void AddArgument(Argument arg);
    void SetAction(Action<Argument[]> action);
    void InvokeAction(in Argument[] args);
}
