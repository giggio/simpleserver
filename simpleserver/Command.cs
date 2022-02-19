using System.CommandLine.Binding;

namespace simpleserver;
public class Commands
{
    public RootCommand CreateRootCommand(string? currentDirectory = null)
    {
        if (currentDirectory == null)
            currentDirectory = Environment.CurrentDirectory;
        var indexOption = new Option<bool>(new[] { "--index", "-i" }, "Use a default index file when a file is not found.") { Arity = ArgumentArity.ZeroOrOne };
        var interfaceOption = new Option<string>(new[] { "--interface" }, () => "0.0.0.0", "Network interface (IP or hostname) to bind the server to.");
        var portOption = new Option<ushort?>(new[] { "--port", "-p" }, () => 8080, "Port to run the server at.");
        var verboseOption = new Option<bool>(new[] { "-v", "--verbose" }, "Show verbose output.") { Arity = ArgumentArity.ZeroOrOne };
        var enableDirectoryBrowsingOption = new Option<bool>(new[] { "--directory-browsing", "-d" }, "Enable directory browsing.") { Arity = ArgumentArity.ZeroOrOne };
        var useSimplePathsOption = new Option<bool>(new[] { "--simple-paths", "-s" }, "Use simple paths. Allows to serve 'file.html' as '/file'.") { Arity = ArgumentArity.ZeroOrOne };
        var logHttpRequestsOption = new Option<bool>(new[] { "-l", "--log-http" }, "Log http output to stdout.") { Arity = ArgumentArity.ZeroOrOne };
        var indexFileNameOption = new Option<string>(new[] { "--index-name" }, () => "index.html", "Default index file name.");
        var pathArgument = new Argument<string>("path", () => currentDirectory, "Directory from where the files will be served.") { Arity = ArgumentArity.ZeroOrOne };
        var rootCommand = new RootCommand("A simple server to run a static backend.")
        {
            pathArgument,
            indexOption,
            interfaceOption,
            portOption,
            indexFileNameOption,
            verboseOption,
            useSimplePathsOption,
            logHttpRequestsOption,
            enableDirectoryBrowsingOption
        };
        rootCommand.SetHandler<Args>(Run, new ArgsBinder(indexOption, interfaceOption, portOption, indexFileNameOption, verboseOption, useSimplePathsOption,
            logHttpRequestsOption, enableDirectoryBrowsingOption, pathArgument));
        return rootCommand;
    }

    public virtual void Run(Args args)
    {
        if (!Path.IsPathRooted(args.Path))
            args = args with { Path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, args.Path)) };
        Environment.CurrentDirectory = args.Path;
        var disabledDirectoryBrowsing = args.UseIndex && args.EnableDirectoryBrowsing;
        if (disabledDirectoryBrowsing)
            args = args with { EnableDirectoryBrowsing = false };
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            WebRootPath = args.Path,
            ContentRootPath = args.Path
        });
        if (args.Verbose)
            builder.Logging.SetMinimumLevel(LogLevel.Information);
        else
            builder.Logging.SetMinimumLevel(LogLevel.Warning);
        if (args.EnableDirectoryBrowsing)
            builder.Services.AddDirectoryBrowser();
        using var app = builder.Build();
        if (disabledDirectoryBrowsing)
            app.Logger.LogWarning($"Disabling directory browsing, as use index is selected.");
        var indexFullFileName = Path.Combine(args.Path, args.IndexFileName);
        if (args.UseIndex && !File.Exists(indexFullFileName))
        {
            args = args with { UseIndex = false };
            app.Logger.LogWarning("Disabling index, as the index file could not be found at '{indexFullFileName}'.", indexFullFileName);
        }
        if (args.LogHttpRequests)
            app.UseHttpLogging();
        if (args.SimplePaths)
        {
            if (args.EnableDirectoryBrowsing)
            {
                args = args with { SimplePaths = false };
                app.Logger.LogWarning("Disabling simple paths as directory browsing is enabled.");
            }
            else
            {
                app.UseSimplePaths();
            }
        }
        app.UseStaticFiles(new StaticFileOptions
        {
            ServeUnknownFileTypes = true,
            DefaultContentType = "text/plain",
        });
        if (args.EnableDirectoryBrowsing)
            app.UseDirectoryBrowser();
        if (args.UseIndex)
            app.UseIndex(new IndexMiddlewareOptions(args.IndexFileName));
        var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
        var appUrl = $"http://{args.Interface}:{args.Port}";
        lifetime.ApplicationStarted.Register(() => Console.WriteLine($"Application running at {appUrl}. Press CTRL+C to stop it."));
        if (args.Verbose)
        {
            app.Logger.LogInformation($@"path {args.Path}
use index {args.UseIndex}
indexFileName {args.IndexFileName}
verbose {args.Verbose}
enableDirectoryBrowsing {args.EnableDirectoryBrowsing}
logHttpRequests {args.LogHttpRequests}
port {args.Port}
interface {args.Interface}
simplePaths {args.SimplePaths}");
        }
        app.Run(appUrl);
        app.Logger.LogInformation("Done!");
    }
}

public class ArgsBinder : BinderBase<Args>
{
    private readonly Option<bool> useIndex;
    private readonly Option<string> @interface;
    private readonly Option<ushort?> port;
    private readonly Option<string> indexFileName;
    private readonly Option<bool> verbose;
    private readonly Option<bool> simplePaths;
    private readonly Option<bool> logHttpRequests;
    private readonly Option<bool> enableDirectoryBrowsing;
    private readonly Argument<string> path;

    public ArgsBinder(Option<bool> useIndex,
                      Option<string> @interface,
                      Option<ushort?> port,
                      Option<string> indexFileName,
                      Option<bool> verbose,
                      Option<bool> simplePaths,
                      Option<bool> logHttpRequests,
                      Option<bool> enableDirectoryBrowsing,
                      Argument<string> path)
    {
        this.useIndex = useIndex;
        this.@interface = @interface;
        this.port = port;
        this.indexFileName = indexFileName;
        this.verbose = verbose;
        this.simplePaths = simplePaths;
        this.logHttpRequests = logHttpRequests;
        this.enableDirectoryBrowsing = enableDirectoryBrowsing;
        this.path = path;
    }

    protected override Args GetBoundValue(BindingContext bindingContext) =>
        new(
            bindingContext.ParseResult.GetValueForOption(useIndex),
            bindingContext.ParseResult.GetValueForOption(@interface)!, // we have a default for this option
            bindingContext.ParseResult.GetValueForOption(port),
            bindingContext.ParseResult.GetValueForOption(indexFileName)!, // we have a default for this option
            bindingContext.ParseResult.GetValueForOption(verbose),
            bindingContext.ParseResult.GetValueForOption(simplePaths),
            bindingContext.ParseResult.GetValueForOption(logHttpRequests),
            bindingContext.ParseResult.GetValueForOption(enableDirectoryBrowsing),
            bindingContext.ParseResult.GetValueForArgument(path)
        );
}

public record Args(bool UseIndex, string Interface, ushort? Port, string IndexFileName, bool Verbose, bool SimplePaths, bool LogHttpRequests, bool EnableDirectoryBrowsing, string Path);
