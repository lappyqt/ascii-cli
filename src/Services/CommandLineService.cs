namespace ascii_cli.Services;

public sealed class CommandLineService : ICommandLineService
{
    private RootCommand _rootCommand;
    private List<SubCommand> _subCommands;
    private ICommand? _activeCommand;

    public CommandLineService(RootCommand rootCommand)
    {
        _rootCommand = rootCommand;
        _subCommands = [];
    }

    public void AddSubCommand(SubCommand subCommand) => _subCommands.Add(subCommand);

    public void ParseArguments(string[] args, out Argument[] parsedArguments)
    {
        _activeCommand = (args.Length > 0 && _subCommands.Any(x => x.Name == args[0]))
            ? _subCommands.First(x => x.Name == args[0])
            : _rootCommand;

        if (args.Length > 0 && _activeCommand is null) throw new NullReferenceException($"Command not found: {args[0]}");
        parsedArguments = ParseCommandArguments(_activeCommand, args);
    }

    public void InvokeCommand(Argument[] parsedArguments) => _activeCommand?.InvokeAction(parsedArguments);

    private Argument[] ParseCommandArguments(ICommand command, string[] args)
    {
        List<Argument> parsedArguments = [];
        Argument[] commandArguments = command.GetArguments();

        for (int i = (command is RootCommand) ? 0 : 1; i < args.Length; i++)
        {
            Argument? arg = commandArguments.FirstOrDefault(x => x.Name == args[i]);

            if (arg is null) throw new Exception($"Argument not found: {args[i]}");

            else if (arg.Type == ArgumentValueType.NoValue)
            {
                parsedArguments.Add(arg);
                continue;
            }

            try {
                arg.Value = args[i + 1];
                parsedArguments.Add(arg);
                i++;
            }
            catch (IndexOutOfRangeException exception) {
                throw new NullReferenceException($"No required value was set for argument {arg.Name}", exception);
            }
        }

        return parsedArguments.ToArray();
    }
}
