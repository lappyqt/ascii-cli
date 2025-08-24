namespace ascii_cli.Services;

public sealed class CommandLineService : ICommandLineService
{
    public ICommand? ActiveCommand { get; private set; }

    private RootCommand _rootCommand;
    private List<SubCommand> _subCommands;
    private ICommand? _activeCommand;

    public CommandLineService(RootCommand rootCommand)
    {
        _rootCommand = rootCommand;
        _subCommands = [];
    }

    public IReadOnlyList<SubCommand> GetSubCommands() => _subCommands.AsReadOnly();

    public void AddSubCommand(SubCommand subCommand) => _subCommands.Add(subCommand);

    /// <summary>
    /// Returns parsed arguments of active-command based on <paramref name="args"/> (Default: RootCommand).
    /// If the only argument is a sub-command: <paramref name="parsedArguments"/> will be empty.
    /// You can get active-command with <see langword="property"/> "ActiveCommand"
    /// </summary>
    public void ParseArguments(string[] args, out Argument[] parsedArguments)
    {
        if (args.Length == 0)
        {
            _activeCommand = _rootCommand;
            parsedArguments = [];
            return;
        }

        _activeCommand = FindCommand(args[0]) ?? FindRootCommandArgument(args[0]);

        this.ActiveCommand = _activeCommand;
        parsedArguments = ParseCommandArguments(_activeCommand, args);
    }

    private ICommand? FindCommand(string arg) => _subCommands.FirstOrDefault(x => x.Name == arg);

    private ICommand FindRootCommandArgument(string arg)
    {
        Argument? argument = _rootCommand.GetArguments().FirstOrDefault(x => x.Name == arg);

        if (argument is not null)
        {
            return _rootCommand;
        }

        throw new NoSuitableOptionFoundException($"Command or argument not found: {arg}");
    }

    public void InvokeCommand(in Argument[] parsedArguments) => _activeCommand?.InvokeAction(parsedArguments);

    private Argument[] ParseCommandArguments(ICommand command, string[] args)
    {
        List<Argument> parsedArguments = [];
        Argument[] commandArguments = command.GetArguments();

        for (int i = (command is RootCommand) ? 0 : 1; i < args.Length; i++)
        {
            Argument? arg = commandArguments.FirstOrDefault(x => x.Name == args[i]);

            if (arg is null)
            {
                throw new ArgumentNotFoundException($"Argument not found: {args[i]}");
            }
            else if (parsedArguments.Any(x => x.Name == arg.Name))
            {
                throw new DuplicateArgumentParseException($"Argument duplicate ({arg.Name})");
            }
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
                throw new NoRequiredArgumentValueException($"No required value was set for argument {arg.Name}", exception);
            }
        }

        return [..parsedArguments];
    }
}
