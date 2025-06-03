
using RestaurantSystem.Utilities;

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
            UserUtility userUtility, RoleService roleService)
        {
            string path = context.Request.Path.Value ?? "/";

            if (path.StartsWith(staff_endpoint_prefix))
            {
                if (path.Contains('?'))
                {
                    path = path.Split('?')[0];
                }

                path = path.Last() == '/' ? path.Remove(path.LastIndexOf('/')) : path;

                Dictionary<string, object>? claims = await userUtility.GetAuthClaimFromJWT(context);

                if (claims == null || !claims.TryGetValue("Id", out object? value) ||
                    !int.TryParse((string?)value, out int Id))
                {
                    return;
                }

                if (!await roleService.CanUserAccessService(Id, path))
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

    public static class AuthenticationMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuthenticationMiddleware(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthenticationMiddleware>();
        }
    }
}
