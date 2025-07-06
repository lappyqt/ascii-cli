using ascii_cli.Services;

RootCommand rootCommand = new RootCommand();
rootCommand.AddArgument(new Argument { Name = "--width", Type = ArgumentValueType.Int });
rootCommand.SetAction(args => {
    Console.WriteLine("Root command used");
});

SubCommand helpCommand = new SubCommand("--help");
helpCommand.SetAction(_ => Console.WriteLine("Help command used"));

CommandLineService commandLineService = new CommandLineService(rootCommand);

commandLineService.AddSubCommand(helpCommand);

commandLineService.ParseArguments(args, out Argument[] parsedArguments);
commandLineService.InvokeCommand(parsedArguments);
