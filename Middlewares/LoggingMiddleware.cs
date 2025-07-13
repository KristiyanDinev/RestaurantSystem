namespace RestaurantSystem.Middlewares
{
    public class LoggingMiddleware
    {

        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ILogger<LoggingMiddleware> logger)
        {
            string? path = context.Request.Path.Value;
            if (path != null && path.StartsWith("/assets"))
            {
                await _next(context);
            }

            logger.LogInformation("Custom Logging:\n");
            logger.LogInformation(context.Request.Method + ": " + path);
            logger.LogInformation("\n----\nCookies:");
            foreach (var cookie in context.Request.Cookies)
            {
                logger.LogInformation(cookie.ToString());
            }
            logger.LogInformation("----\n");

            await _next(context);
        }
    }

    public static class LoggingMiddlewareExtension
    {
        public static IApplicationBuilder UseLoggingMiddleware(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LoggingMiddleware>();
        }
    }
}
