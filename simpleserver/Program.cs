using System.CommandLine;
using System.CommandLine.Binding;

var indexOption = new Option<bool>(new[] { "--index", "-i" }, "desc") { Arity = ArgumentArity.ZeroOrOne };
var interfaceOption = new Option<string>(new[] { "--interface" }, () => "0.0.0.0", "desc");
var portOption = new Option<int>(new[] { "--port", "-p" }, () => 8080, "desc");
var verboseOption = new Option<bool>(new[] { "-v", "--verbose" }, "desc") { Arity = ArgumentArity.ZeroOrOne };
var enableDirectoryBrowsingOption = new Option<bool>(new[] { "--directory-browsing", "-d" }, "desc") { Arity = ArgumentArity.ZeroOrOne };
var useSimplePathsOption = new Option<bool>(new[] { "--simple-paths", "-s" }, "desc") { Arity = ArgumentArity.ZeroOrOne };
var logHttpRequestsOption = new Option<bool>(new[] { "-l", "--log-http" }, "desc") { Arity = ArgumentArity.ZeroOrOne };
var indexFileNameOption = new Option<string>(new[] { "--index-name" }, () => "index.html", "desc");
var path = new Argument<string>("path", () => Environment.CurrentDirectory, "desc") { Arity = ArgumentArity.ZeroOrOne };
var rootCommand = new RootCommand
{
    path,
    indexOption,
    interfaceOption,
    portOption,
    indexFileNameOption,
    verboseOption,
    useSimplePathsOption,
    logHttpRequestsOption,
    enableDirectoryBrowsingOption
};
rootCommand.SetHandler((bool useIndex, string @interface, int port, string indexFileName, bool verbose, bool simplePaths, bool logHttpRequests, bool enableDirectoryBrowsing, string path) =>
{
    Environment.CurrentDirectory = Path.IsPathRooted(path) ? path : Path.Combine(Environment.CurrentDirectory, path);
    var disabledDirectoryBrowsing = useIndex && enableDirectoryBrowsing;
    if (disabledDirectoryBrowsing)
        enableDirectoryBrowsing = false;
    var builder = WebApplication.CreateBuilder(new WebApplicationOptions
    {
        WebRootPath = path,
        ContentRootPath = path
    });
    if (enableDirectoryBrowsing)
        builder.Services.AddDirectoryBrowser();
    var app = builder.Build();
    if (disabledDirectoryBrowsing)
        app.Logger.LogWarning($"Disabling directory browsing, as use index is selected.");
    var indexFullFileName = Path.Combine(path, indexFileName);
    if (useIndex && !File.Exists(indexFullFileName))
    {
        useIndex = false;
        app.Logger.LogWarning("Disabling index, as the index file could not be found at '{indexFullFileName}'.", indexFullFileName);
    }
    if (verbose)
    {
        app.Logger.LogInformation($@"path {path}
use index {useIndex}
indexFileName {indexFileName}
verbose {verbose}
enableDirectoryBrowsing {enableDirectoryBrowsing}
logHttpRequests {logHttpRequests}
port {port}
interface {@interface}
simplePaths {simplePaths}");
    }
    if (logHttpRequests)
        app.UseHttpLogging();
    if (!enableDirectoryBrowsing && simplePaths)
        app.UseSimplePaths();
    app.UseStaticFiles(new StaticFileOptions
    {
        ServeUnknownFileTypes = true,
        DefaultContentType = "text/plain",
    });
    if (enableDirectoryBrowsing)
        app.UseDirectoryBrowser();
    if (useIndex)
        app.UseIndex(new IndexMiddlewareOptions(indexFileName));
    app.Run($"http://{@interface}:{port}");
    app.Logger.LogInformation("Done!");
}, rootCommand.Children.OfType<IValueDescriptor>().ToArray());
rootCommand.Invoke(args);
