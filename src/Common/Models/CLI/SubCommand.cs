namespace ascii_cli.Common.Models.CLI;

public sealed class SubCommand : ICommand
{
    public required string Name { get; init; }

    public SubCommand(string name)
    {
        Name = name;
    }

    public void AddArgument(Argument arg)
    {
        throw new NotImplementedException();
    }

    public Argument[] GetArguments()
    {
        throw new NotImplementedException();
    }

    public void SetAction(Action action)
    {
        throw new NotImplementedException();
    }

    public void SetAction(Action<Argument[]> action)
    {
        throw new NotImplementedException();
    }

    public void InvokeAction(Argument[] args)
    {
        throw new NotImplementedException();
    }
}
