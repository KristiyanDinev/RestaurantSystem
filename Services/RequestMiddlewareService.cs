
namespace ITStepFinalProject.Services
{
    public class RequestMiddlewareService
    {

        public RequestMiddlewareService() { }

        public void RegisterMiddlewarePerService(WebApplication app, 
            AuthenticationService authenticationService)
        {
            app.Use(async (HttpContext context, RequestDelegate next) => {

                Console.WriteLine("\n----\nCookies:");
                foreach (var cookie in context.Request.Cookies)
                {
                    Console.WriteLine(cookie);
                }
                Console.WriteLine("----\n");


                // context.Request.Path.Value something like: /login/login.js
                string? path = context.Request.Path.Value;
                Console.WriteLine(context.Request.Method + " " + path);

                if (await authenticationService.HandleUserAuthentication(context))
                {
                    return;
                }

                await next.Invoke(context);
            });
        }
    }
}
