
using Microsoft.AspNetCore.Identity;
using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Services
{
    public class AuthenticationMiddleware
    {
        private readonly string staff_endpoint_prefix = "/staff";

        private readonly RequestDelegate _next;
        public AuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, 
            UserManager<UserModel> userManager, RoleService roleService)
        {
            string path = context.Request.Path.Value ?? "/";
            if (path.StartsWith(staff_endpoint_prefix))
            {
                if (path.Contains('?'))
                {
                    path = path.Split('?')[0];
                }
                path = path.Last() == '/' ? path.Remove(path.LastIndexOf('/')) : path;
                
                UserModel? user = await userManager.GetUserAsync(context.User);
                if (user == null)
                {
                    context.Response.Redirect("/login");
                    return;
                }
                if (!await roleService.CanUserAccessServiceAsync(user.Id, path))
                {
                    // user doesn't have the roles to do so.
                    context.Response.Redirect("/login");
                    return;
                }
            }

            // Call the next delegate/middleware in the pipeline.
            await _next(context);
        }
    }

    public static class AuthenticationMiddlewareExtension
    {
        public static IApplicationBuilder UseAuthenticationMiddleware(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthenticationMiddleware>();
        }
    }
}
