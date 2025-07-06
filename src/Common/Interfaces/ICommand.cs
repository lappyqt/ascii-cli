namespace ascii_cli.Common.Interfaces;

public interface ICommand
{
    void AddArgument(Argument arg);
    Argument[] GetArguments();
    void SetAction(Action<Argument[]> action);
    void InvokeAction(Argument[] args);
}
