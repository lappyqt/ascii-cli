namespace ascii_cli.Common.Models.CLI;

public sealed class SubCommand : ICommand
{
    private List<Argument> _arguments = new ();
    private Action<Argument[]>? _action;

    public string Name { get; init; }

    public SubCommand(string name)
    {
        Name = name;
    }

    public void AddArgument(Argument arg)
    {
        if (_arguments.Contains(arg)) throw new DuplicateWaitObjectException("Current argument has already been added to the list.");
        _arguments.Add(arg);
    }

    public Argument[] GetArguments() => _arguments.ToArray();
    public void SetAction(Action<Argument[]> action) => _action = action;
    public void InvokeAction(Argument[] args) => _action?.Invoke(args);
}
