using RestaurantSystem.Models.DatabaseModels;
using RestaurantSystem.Utils;

namespace RestaurantSystem.Services
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

        public async Task InvokeAsync(HttpContext context, UserUtils userUtils, 
            RoleService roleService)
        {
            string path = context.Request.Path.Value ?? "/";
            if (path.Equals('/'))
            {
                return;
            }

            bool isAdminEndpoint = ("/"+path.Split("/")[0]).Equals(admin_endpoint_prefix);
            UserModel? user = await userUtils.GetUserByJWT(context);

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

            if (isAdminEndpoint && user != null)
            {
                // the user is logged in
                if (path.Contains('?'))
                {
                    path = path.Split('?')[0];
                }
                List<string> splited = path.Split("/").ToList();
                splited.RemoveAt(0);
                path = "/" + string.Join("/", splited);
                if (!await roleService.CanUserAccessService(user.Id, path))
                {
                    // user doesn't have the roles to do so.
                    Console.WriteLine("\n"+user.Name +" was trying to reach to admin page. Service: "+
                        path+" without the proper roles.\n");
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
