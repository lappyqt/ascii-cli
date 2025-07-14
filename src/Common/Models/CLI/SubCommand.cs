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
        if (_arguments.Any(x => x.Name == arg.Name)) throw new DuplicateArgumentInitException($"Argument {arg.Name} has already been added to the list.");
        _arguments.Add(arg);
    }

    public Argument[] GetArguments() => _arguments.ToArray();
    public void SetAction(Action<Argument[]> action) => _action = action;
    public void InvokeAction(in Argument[] args) => _action?.Invoke(args);
}
