using System.CommandLine.Builder;
using System.CommandLine.Parsing;

var rootCommand = new Commands().CreateRootCommand(Environment.CurrentDirectory);
var parser = new CommandLineBuilder(rootCommand)
    .UseDefaults()
    .Build();
return parser!.Invoke(args);
