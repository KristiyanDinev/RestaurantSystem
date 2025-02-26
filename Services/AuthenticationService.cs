using ITStepFinalProject.Database.Handlers;
using ITStepFinalProject.Models;
using ITStepFinalProject.Utils;
using System;

namespace ITStepFinalProject.Services
{
    public class AuthenticationService
    {
        private readonly List<string> non_login_endpoints = 
            ["/login", "/register"];

        private ControllerUtils _controllerUtils;
        private UserUtils _userUtils;
        private UserDatabaseHandler _userDatabaseHandlder;
        public AuthenticationService(ControllerUtils controllerUtils,
            UserDatabaseHandler userDatabaseHandlder,
            UserUtils userUtils)
        {
            _controllerUtils = controllerUtils;
            _userDatabaseHandlder = userDatabaseHandlder;
            _userUtils = userUtils;
        }

        public async Task<bool> HandleUserAuthentication(HttpContext context)
        {
            string path = context.Request.Path.Value ?? "/";
            if (path.Length.Equals('/'))
            {
                return false;
            }

            UserModel? user = await _userUtils.GetUserModelFromAuth(context);
            if (user != null && non_login_endpoints.Contains(path))
            {
                // user is logged in and tries to login in.
                context.Response.Redirect("/dishes");
                return true;

            } else if (user == null && !non_login_endpoints.Contains(path))
            {
                // user is not logged in and it tries to visit an endpoint that requires login
                context.Response.Redirect("/login");
                return true;
            }
            return false;
        }
    }
}
