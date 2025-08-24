namespace ascii_cli.Common.Models.CLI;

public sealed class RootCommand : ICommand
{
    public string Name { get => "Root"; }

    private List<Argument> _arguments = [];
    private Action<Argument[]>? _action;

    public void AddArgument(Argument arg)
    {
        if (_arguments.Any(x => x.Name == arg.Name))
        {
            throw new DuplicateArgumentInitException($"Argument {arg.Name} has already been added to the list.");
        }

        _arguments.Add(arg);
    }

    public Argument[] GetArguments() => [.._arguments];
    public void SetAction(Action<Argument[]> action) => _action = action;
    public void InvokeAction(in Argument[] args) => _action?.Invoke(args);
}
