namespace ascii_cli.Common.Interfaces;

public interface ICommandLineService
{
    void AddRootCommand(RootCommand rootCommand);
    void AddSubCommand(SubCommand subCommand);
}
