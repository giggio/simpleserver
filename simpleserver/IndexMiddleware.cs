using Microsoft.Extensions.Options;

namespace simpleserver;
public class IndexMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger<IndexMiddleware> logger;
    private readonly string defaultFileContents;
    private readonly int fileLength;

    public IndexMiddleware(RequestDelegate next, ILogger<IndexMiddleware> logger, IndexMiddlewareOptions options)
    {
        this.next = next ?? throw new ArgumentNullException(nameof(next));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        defaultFileContents = File.ReadAllText(options.IndexFileName);
        fileLength = defaultFileContents.Length;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        logger.LogInformation("Serving index file.");
        context.Response.ContentLength = fileLength;
        context.Response.ContentType = "text/html";
        context.Response.StatusCode = 200;
        await context.Response.WriteAsync(defaultFileContents);
    }
}

public static class IndexMiddlewareExtensions
{
    public static IApplicationBuilder UseIndex(this IApplicationBuilder builder, IndexMiddlewareOptions options) =>
        builder.UseMiddleware<IndexMiddleware>(options);
}

public class IndexMiddlewareOptions
{
    public string IndexFileName { get; set; }

    public IndexMiddlewareOptions(string indexFileName)
    {
        IndexFileName = indexFileName;
    }
}
