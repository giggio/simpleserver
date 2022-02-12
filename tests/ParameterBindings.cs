namespace tests;

public class ParameterBindings
{
    private readonly Commands commands = Substitute.ForPartsOf<Commands>();
    private readonly TestConsole console = new();
    private readonly RootCommand command;

    public ParameterBindings()
    {
        commands.WhenForAnyArgs(x => x.Run(Arg.Any<Args>())).DoNotCallBase();
        command = commands.CreateRootCommand("/apath");
    }

    [Fact]
    public void CheckBindingWithoutArgs()
    {
        command.Invoke("", console);
        commands.Received().Run(Arg.Is<Args>(arg =>
            !arg.EnableDirectoryBrowsing
            && arg.IndexFileName == "index.html"
            && arg.Interface == "0.0.0.0"
            && !arg.LogHttpRequests
            && arg.Path == "/apath"
            && arg.Port == 8080
            && !arg.SimplePaths
            && !arg.UseIndex
            && !arg.Verbose
        ));
    }

    [Fact]
    public void CheckBindingDirectoryBrowsing()
    {
        command.Invoke("-d", console);
        commands.Received().Run(Arg.Is<Args>(arg => arg.EnableDirectoryBrowsing));
    }

    [Fact]
    public void CheckBindingDirectoryBrowsingLong()
    {
        command.Invoke("--directory-browsing", console);
        commands.Received().Run(Arg.Is<Args>(arg => arg.EnableDirectoryBrowsing));
    }

    [Fact]
    public void CheckBindingIndexFileName()
    {
        command.Invoke(new[] { "--index-name", "foo.htm" }, console);
        commands.Received().Run(Arg.Is<Args>(arg => arg.IndexFileName == "foo.htm"));
    }

    [Fact]
    public void CheckBindingInterface()
    {
        command.Invoke(new[] { "--interface", "myhost.com" }, console);
        commands.Received().Run(Arg.Is<Args>(arg => arg.Interface == "myhost.com"));
    }

    [Fact]
    public void CheckBindingLogHttpRequests()
    {
        command.Invoke("-l", console);
        commands.Received().Run(Arg.Is<Args>(arg => arg.LogHttpRequests));
    }

    [Fact]
    public void CheckBindingLogHttpRequestsLong()
    {
        command.Invoke("--log-http", console);
        commands.Received().Run(Arg.Is<Args>(arg => arg.LogHttpRequests));
    }

    [Fact]
    public void CheckBindingPath()
    {
        command.Invoke("/tmp/foo", console);
        commands.Received().Run(Arg.Is<Args>(arg => arg.Path == "/tmp/foo"));
    }

    [Fact]
    public void CheckBindingPort()
    {
        command.Invoke(new[] { "-p", "1000" }, console);
        commands.Received().Run(Arg.Is<Args>(arg => arg.Port == 1000));
    }

    [Fact]
    public void CheckBindingPortLong()
    {
        command.Invoke(new[] { "--port", "1000" }, console);
        commands.Received().Run(Arg.Is<Args>(arg => arg.Port == 1000));
    }

    [Fact]
    public void CheckBindingSimplesPaths()
    {
        command.Invoke("-s", console);
        commands.Received().Run(Arg.Is<Args>(arg => arg.SimplePaths));
    }

    [Fact]
    public void CheckBindingSimplesPathsLong()
    {
        command.Invoke("--simple-paths", console);
        commands.Received().Run(Arg.Is<Args>(arg => arg.SimplePaths));
    }

    [Fact]
    public void CheckBindingUseIndex()
    {
        command.Invoke("-i", console);
        commands.Received().Run(Arg.Is<Args>(arg => arg.UseIndex));
    }

    [Fact]
    public void CheckBindingUseIndexLong()
    {
        command.Invoke("--index", console);
        commands.Received().Run(Arg.Is<Args>(arg => arg.UseIndex));
    }

    [Fact]
    public void CheckBindingVerbose()
    {
        command.Invoke(new[] { "-v" }, console);
        commands.Received().Run(Arg.Is<Args>(arg => arg.Verbose));
    }

    [Fact]
    public void CheckBindingVerboseLong()
    {
        command.Invoke(new[] { "--verbose" }, console);
        commands.Received().Run(Arg.Is<Args>(arg => arg.Verbose));
    }
}