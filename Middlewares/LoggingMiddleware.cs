namespace RestaurantSystem.Middlewares
{
    public class LoggingMiddleware
    {

        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string? path = context.Request.Path.Value;
            if (path != null && path.StartsWith("/assets"))
            {
                await _next(context);
            }

            Console.WriteLine("Custom Logging:\n");
            Console.WriteLine(context.Request.Method + ": " + path);
            Console.WriteLine("\n----\nCookies:");
            foreach (var cookie in context.Request.Cookies)
            {
                Console.WriteLine(cookie);
            }
            Console.WriteLine("----\n");

            await _next(context);
        }
    }

    public static class LoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseLoggingMiddleware(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LoggingMiddleware>();
        }
    }
}
