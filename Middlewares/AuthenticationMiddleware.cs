using ITStepFinalProject.Models.DatabaseModels;
using ITStepFinalProject.Utils.Controller;

namespace ITStepFinalProject.Services
{
    public class AuthenticationMiddleware
    {
        private readonly List<string> non_login_endpoints = 
            ["/login", "/register"];

        private readonly RequestDelegate _next;
        public AuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, UserUtils _userUtils)
        {
            string path = context.Request.Path.Value ?? "/";
            if (path.Length.Equals('/'))
            {
                return;
            }

            UserModel? user = await _userUtils.GetUserModelFromAuth(context);
            if (user != null && non_login_endpoints.Contains(path))
            {
                // user is logged in and tries to login in.
                context.Response.Redirect("/dishes");
                return;

            }
            else if (user == null && !non_login_endpoints.Contains(path))
            {
                // user is not logged in and it tries to visit an endpoint that requires login
                context.Response.Redirect("/login");
                return;
            }

            // Call the next delegate/middleware in the pipeline.
            await _next(context);
        }
    }

    public static class AuthenticationMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuthenticationMiddleware(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthenticationMiddleware>();
        }
    }
}
