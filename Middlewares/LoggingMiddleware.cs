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
            Console.WriteLine("Custom Logging:\n");
            Console.WriteLine("\n" + context.Request.Method + ": " + context.Request.Path.Value);
            Console.WriteLine("\n----\nCookies:");
            foreach (var cookie in context.Request.Cookies)
            {
                Console.WriteLine(cookie);
            }
            Console.WriteLine("----\n");

            Console.WriteLine("End of custom Logging:\n");

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
