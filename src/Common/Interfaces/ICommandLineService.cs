namespace ascii_cli.Common.Interfaces;

public interface ICommandLineService
{
    ICommand? ActiveCommand { get; }

    IReadOnlyList<SubCommand> GetSubCommands();
    void AddSubCommand(SubCommand subCommand);
    void ParseArguments(string[] args, out Argument[] parsedArguments);
    void InvokeCommand(in Argument[] parsedArguments);
}
