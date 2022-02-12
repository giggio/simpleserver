namespace simpleserver;
public class SimplePathsMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger<SimplePathsMiddleware> logger;

    public SimplePathsMiddleware(RequestDelegate next, ILogger<SimplePathsMiddleware> logger) =>
        (this.next, this.logger) = (next, logger);

    public Task InvokeAsync(HttpContext context)
    {
        if (System.Text.RegularExpressions.Regex.IsMatch(context.Request.Path.ToString(), @"^([^.]+)$"))
        {
            context.Request.Path += ".html";
            logger.LogInformation("Path now is {path}", context.Request.Path);
        }
        return next(context);
    }
}

public static class SimplePathsMiddlewareExtensions
{
    public static IApplicationBuilder UseSimplePaths(this IApplicationBuilder builder) =>
        builder.UseMiddleware<SimplePathsMiddleware>();
}
