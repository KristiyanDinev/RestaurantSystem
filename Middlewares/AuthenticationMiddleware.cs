using ITStepFinalProject.Database.Handlers;
using ITStepFinalProject.Models.DatabaseModels;
using ITStepFinalProject.Utils.Controller;

namespace ITStepFinalProject.Services
{
    public class AuthenticationMiddleware
    {
        private readonly List<string> non_login_endpoints = 
            ["/login", "/register"];

        private readonly string admin_endpoint_prefix = "/admin";

        private readonly RequestDelegate _next;
        public AuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, UserUtils _userUtils, 
            ServiceDatabaseHandler serviceDatabaseHandler, UserDatabaseHandler userDatabaseHandler)
        {
            string path = context.Request.Path.Value ?? "/";
            if (path.Equals('/'))
            {
                return;
            }

            bool isAdminEndpoint = path.StartsWith(admin_endpoint_prefix);
            UserModel? user = await _userUtils.GetLoginUserFromCookie(context, userDatabaseHandler);

            if (user != null && non_login_endpoints.Contains(path))
            {
                // user is logged in and tries to login in.
                context.Response.Redirect("/dishes");
                return;

            }
            else if (user == null && (!non_login_endpoints.Contains(path) || isAdminEndpoint))
            {
                // user is not logged in and it tries to visit an endpoint that requires login
                context.Response.Redirect("/login");
                return;

            }

            if (isAdminEndpoint)
            {
                // the user is logged in
                if (path.Contains('?'))
                {
                    path = path.Split('?')[0];
                }
                path = path.Substring(admin_endpoint_prefix.Length);
                if (!await serviceDatabaseHandler.DoesUserHaveRolesToAccessService(user, path))
                {
                    // user doesn't have the roles to do so.
                    Console.WriteLine(user.Username +" was trying to reach to admin page. Service: "+
                        path+" without the proper roles.");
                    context.Response.Redirect("/dishes");
                    return;
                }
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
