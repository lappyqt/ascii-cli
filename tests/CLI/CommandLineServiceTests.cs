using ascii_cli.Common.Interfaces;
using ascii_cli.Common.Models.CLI;
using ascii_cli.Common.Enums;
using ascii_cli.Common.Exceptions;
using ascii_cli.Services;
using ascii_cli_tests.Helpers;

namespace ascii_cli_tests.CLI;

public class CommandLineServiceTests
{
    private readonly RootCommand _rootCommand;
    private readonly ICommandLineService _commandLineService;

    public CommandLineServiceTests()
    {
        _rootCommand = new RootCommand();
        _commandLineService = new CommandLineService(_rootCommand);
    }

    [Fact]
    public void AddSubCommand_ShouldAddCommandToList()
    {
        SubCommand subCommand = new SubCommand("--help");
        _commandLineService.AddSubCommand(subCommand);
        Assert.Contains(subCommand, _commandLineService.GetSubCommands());
    }

    [Fact]
    public void InvokeCommand_ShouldWork()
    {
        CommandInvocationTracker tracker = new ();
        _rootCommand.SetAction(args => { tracker.WasCommandInvoked = true; });

        _commandLineService.ParseArguments([], out Argument[] parsedArguments);
        _commandLineService.InvokeCommand(parsedArguments);

        Assert.True(tracker.WasCommandInvoked);
    }

    [Fact]
    public void ParseArguments_WithNoArguments_ShouldReturnExpectedConsoleOutput()
    {
        StringWriter consoleOutput = new ();
        Console.SetOut(consoleOutput);

        string expectedConsoleOutput = "root";
        _rootCommand.SetAction(args => Console.Write("root"));

        _commandLineService.ParseArguments([], out Argument[] parsedArguments);
        _commandLineService.InvokeCommand(parsedArguments);

        Assert.Equal(expectedConsoleOutput, consoleOutput.ToString());
    }

    [Fact]
    public void ParseArguments_WithNoArguments_ShouldReturnEmptyArray()
    {
        _commandLineService.ParseArguments([], out Argument[] parsedArguments);
        Assert.Empty(parsedArguments);
    }

    [Theory]
    [InlineData("--folder", "$HOME/Downloads")]
    [InlineData("--colors", "255")]
    [InlineData("--colors", "16", "--list")]
    public void ParseArguments_ShouldParseRootCommandArguments(params string[] args)
    {
        _rootCommand.AddArgument(new Argument { Name = "--list", Type = ArgumentValueType.NoValue });
        _rootCommand.AddArgument(new Argument { Name = "--folder", Type = ArgumentValueType.String });
        _rootCommand.AddArgument(new Argument { Name = "--colors", Type = ArgumentValueType.Int });

        _commandLineService.ParseArguments(args, out Argument[] parsedArgument);

        Assert.Equal(args[0], parsedArgument[0].Name);
        Assert.Equal(args[1], parsedArgument[0].Value);
    }

    [Fact]
    public void ParseArguments_ShouldCorrectlyBindValuesWithNoValueArgumentsInList()
    {
        _rootCommand.AddArgument(new Argument { Name = "--log", Type = ArgumentValueType.NoValue });
        _rootCommand.AddArgument(new Argument { Name = "--silent", Type = ArgumentValueType.NoValue });
        _rootCommand.AddArgument(new Argument { Name = "--outputFolder", Type = ArgumentValueType.String });
        _rootCommand.AddArgument(new Argument { Name = "--copies", Type = ArgumentValueType.Int });

        _commandLineService.ParseArguments(["--log", "--outputFolder", "$HOME/Pictures", "--silent", "--copies", "2"], out Argument[] parsedArguments);

        Assert.Null(parsedArguments.First().Value);
        Assert.Equal("$HOME/Pictures", parsedArguments.FirstOrDefault(x => x.Name == "--outputFolder")!.Value);
        Assert.Equal("2", parsedArguments.FirstOrDefault(x => x.Name == "--copies")!.Value);
    }

    [Fact]
    public void ParseArguments_WithSingleSubCommand_ShouldReturnEmptyArray()
    {
        SubCommand subCommand = new SubCommand("diff");

        _commandLineService.AddSubCommand(subCommand);
        _commandLineService.ParseArguments(["diff"], out Argument[] parsedArguments);

        Assert.Empty(parsedArguments);
    }

    [Theory]
    [InlineData("help")]
    [InlineData("remove")]
    [InlineData("add")]
    public void ParseArguments_WithSingleSubCommand_ShouldReturnCorrectActiveCommand(string subCommandName)
    {
        SubCommand subCommand = new SubCommand(subCommandName);

        _commandLineService.AddSubCommand(subCommand);
        _commandLineService.ParseArguments([subCommandName], out Argument[] parsedArguments);

        Assert.NotNull(_commandLineService.ActiveCommand);
        Assert.Equal(subCommandName, _commandLineService.ActiveCommand.Name);
    }

    [Theory]
    [InlineData("new", "image.jpg")]
    [InlineData("delete", "abc.png")]
    [InlineData("transparency", "50%")]
    public void ParseArguments_WithSingleSubCommand_ShouldReturnExpectedConsoleOutput(string subCommandName, string commandConsoleOutput)
    {
        StringWriter actualConsoleOutput = new ();
        Console.SetOut(actualConsoleOutput);

        SubCommand subCommand = new SubCommand(subCommandName);
        subCommand.SetAction(args => Console.Write(commandConsoleOutput));

        _commandLineService.AddSubCommand(subCommand);
        _commandLineService.ParseArguments([subCommandName], out Argument[] parsedArguments);
        _commandLineService.InvokeCommand(parsedArguments);

        Assert.Equal(commandConsoleOutput, actualConsoleOutput.ToString());
    }

    [Theory]
    [InlineData("--saturation", "123")]
    [InlineData("--quality", "34")]
    [InlineData("--quality", "-54")]
    public void ParseArguments_ShouldParseSubCommandArguments(string argument, string value)
    {
        SubCommand subCommand = new SubCommand("set");
        subCommand.AddArgument(new Argument { Name = "--saturation", Type = ArgumentValueType.Int });
        subCommand.AddArgument(new Argument { Name = "--quality", Type = ArgumentValueType.Int });

        _commandLineService.AddSubCommand(subCommand);
        _commandLineService.ParseArguments(["set", argument, value], out Argument[] parsedArgumets);

        Assert.Equal(value, parsedArgumets[0].Value);
    }

    [Theory]
    [InlineData("Correct")]
    [InlineData("$^$GS")]
    [InlineData("incorrect")]
    public void ParseArguments_ShouldThrowIfCommandOrArgumentNotFound(string commandName)
    {
        SubCommand subCommand = new SubCommand("correct");

        _commandLineService.AddSubCommand(subCommand);

        Assert.Throws<NoSuitableOptionFoundException>(() =>
            _commandLineService.ParseArguments([commandName], out Argument[] parsedArguments)
        );
    }

    [Theory]
    [InlineData("-l")]
    [InlineData("--human-readable")]
    [InlineData("--save")]
    public void ParseArguments_WithSubCommand_ShouldThrowIfArgumentNotFound(string argumentName)
    {
        SubCommand subCommand = new SubCommand("abc");
        subCommand.AddArgument(new Argument { Name = "--list", Type = ArgumentValueType.NoValue });

        _commandLineService.AddSubCommand(subCommand);

        Assert.Throws<ArgumentNotFoundException>(() => {
            _commandLineService.ParseArguments(["abc", argumentName], out Argument[] parsedArguments);
        });
    }

    [Fact]
    public void ParseArguments_ShouldThrowIfDuplicateInitCommandPresented()
    {
        Assert.Throws<DuplicateArgumentInitException>(() => {
            _rootCommand.AddArgument(new Argument { Name = "--tree", Type = ArgumentValueType.String });
            _rootCommand.AddArgument(new Argument { Name = "--tree", Type = ArgumentValueType.String });
        });
    }

    [Theory]
    [InlineData("--name", "ProjectOne")]
    [InlineData("--format", ".png")]
    public void ParseArguments_ShouldThrowIfDuplicateParseCommandPresented(string argumentName, string value)
    {
        _rootCommand.AddArgument(new Argument { Name = "--name", Type = ArgumentValueType.String });
        _rootCommand.AddArgument(new Argument { Name = "--format", Type = ArgumentValueType.String });

        Assert.Throws<DuplicateArgumentParseException>(() => {
             _commandLineService.ParseArguments([argumentName, value, argumentName, value], out Argument[] parsedArguments);
        });
    }

    [Theory]
    [InlineData("--size", "345", "--list", "--name")]
    [InlineData("--list", "--name", "Project", "--size")]
    [InlineData("--name", "Item", "--size")]
    [InlineData("--name")]
    public void ParseArguments_ShouldThrowIfNoRequiredValuePresented(params string[] args)
    {
        _rootCommand.AddArgument(new Argument { Name = "--size", Type = ArgumentValueType.Int });
        _rootCommand.AddArgument(new Argument { Name = "--name", Type = ArgumentValueType.String });
        _rootCommand.AddArgument(new Argument { Name = "--list", Type = ArgumentValueType.NoValue });

        Assert.Throws<NoRequiredArgumentValueException>(() => {
            _commandLineService.ParseArguments(args, out Argument[] parsedArguments);
        });
    }
}
