namespace tests;
public class BasicConfiguration
{
    [Fact]
    public void ParserIsConfiguredCorrectly()
    {
        var command = new Commands().CreateRootCommand();
        var configuration = new CommandLineConfiguration(command);
        configuration.ThrowIfInvalid();
    }
    [Fact]
    public void CheckBinding()
    {
        var commands = Substitute.ForPartsOf<Commands>();
        commands.WhenForAnyArgs(x => x.Run(Arg.Any<Args>())).DoNotCallBase();
        var command = commands.CreateRootCommand();
        var console = new TestConsole();
        command.Invoke("-i", console);
        commands.Received().Run(Arg.Is<Args>(arg => arg.UseIndex));
    }

    [Fact]
    public void DescriptionIsCorrect()
    {
        var command = new Commands().CreateRootCommand("/foo");
        command.Name = "testcommand";
        var console = new TestConsole();
        command.Invoke("-h", console);
        console.Out.ToString()!.Trim().ShouldBe(@"Description:
  A simple server to run a static backend.

Usage:
  testcommand [<path>] [options]

Arguments:
  <path>  Directory from where the files will be served. [default: /foo]

Options:
  -i, --index                Use a default index file when a file is not found.
  --interface <interface>    Network interface (IP or hostname) to bind the server to. [default: 0.0.0.0]
  -p, --port <port>          Port to run the server at. [default: 8080]
  --index-name <index-name>  Default index file name. [default: index.html]
  -v, --verbose              Show verbose output.
  -s, --simple-paths         Use simple paths. Allows to serve 'file.html' as '/file'.
  -l, --log-http             Log http output to stdout.
  -d, --directory-browsing   Enable directory browsing.
  --version                  Show version information
  -?, -h, --help             Show help and usage information", StringCompareShould.IgnoreLineEndings);
    }
}
