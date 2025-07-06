namespace ascii_cli.Common.Interfaces;

public interface ICommandLineService
{
    void AddSubCommand(SubCommand subCommand);
    void ParseArguments(string[] args, out Argument[] parsedArguments);
    void InvokeCommand(Argument[] parsedArguments);
}
