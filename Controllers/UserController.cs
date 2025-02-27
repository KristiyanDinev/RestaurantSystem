using ITStepFinalProject.Database.Handlers;
using Microsoft.AspNetCore.Mvc;
using ITStepFinalProject.Utils.Web;
using ITStepFinalProject.Utils.Controller;
using ITStepFinalProject.Models.DatabaseModels;
using ITStepFinalProject.Models.DatabaseModels.ModifingDatabaseModels;
using ITStepFinalProject.Models.Controller;

namespace ITStepFinalProject.Controllers {
    public class UserController {

        // testing user:
        // email: some@email.com
        // pass: 123

        public UserController(WebApplication app) {

            app.MapGet("/", (HttpContext context) => {
                return Results.Redirect("/login");
            });



            // get front-end logged in user profile (full page)
            app.MapGet("/profile",
                async (HttpContext context, UserDatabaseHandler db,
                ControllerUtils controllerUtils, UserUtils userUtils, WebUtils webUtils) => {

                    return await controllerUtils.HandleDefaultPage_WithUserModel("/profile",
                        context, userUtils, webUtils);

            }).RequireRateLimiting("fixed");



            // get front-end login page
            app.MapGet("/login", async (HttpContext context, ControllerUtils controllerUtils, 
                UserDatabaseHandler db, UserUtils userUtils) => {
                    try {
                        UserModel? user = await userUtils.GetLoginUserFromCookie(context, db);
                        string data = await controllerUtils.GetHTMLFromWWWROOT("/login");

                        return user == null ? Results.Content(data, "text/html") :
                            Results.Redirect("/dishes");

                    } catch (Exception) {
                        return Results.Redirect("/error");
                    }

            }).RequireRateLimiting("fixed");


            // try loging in
            app.MapPost("/login", async (UserDatabaseHandler db, HttpContext context,
                ControllerUtils controllerUtils, UserUtils userUtils,
                [FromForm] string email, [FromForm] string password,
                [FromForm] string rememberMe) => {

                    if (string.IsNullOrWhiteSpace(email) ||
                        string.IsNullOrWhiteSpace(password)) {
                        return Results.BadRequest();
                    }

                    try {
                        UserModel? _user = await userUtils.GetUserModelFromAuth(context);
                        if (_user != null)
                        {
                            return Results.Redirect("/dishes");
                        }

                        UserModel user = new UserModel(email, password);
                        user = await db.LoginUser(user, true) 
                            ?? throw new Exception("Didn't login");

                        string authString = userUtils.HandleAuth(user, rememberMe);

                        context.Response.Cookies.Delete(userUtils.authHeader);
                        context.Response.Cookies.Append(userUtils.authHeader, authString);

                        return Results.Ok();

                    } catch (Exception) {
                        return Results.Unauthorized();
                    }

            }).RequireRateLimiting("fixed")
            .DisableAntiforgery();


            // get front-end register page
            app.MapGet("/register", async (HttpContext context, ControllerUtils controllerUtils, 
                UserDatabaseHandler db, UserUtils userUtils) => {
                try {
                    UserModel? user = await userUtils.GetLoginUserFromCookie(context, db);
                    string data = await controllerUtils.GetHTMLFromWWWROOT("/register");

                    return user == null ? Results.Content(data, "text/html") :
                        Results.Redirect("/dishes");

                } catch (Exception) {
                    return Results.Redirect("/error");
                }

            }).RequireRateLimiting("fixed");



            // try registering
            app.MapPost("/register", async (UserDatabaseHandler db, HttpContext context,
                ControllerUtils controllerUtils, UserUtils userUtils,
                [FromForm] RegisterUserModel registerUserModel) => {

                    if (string.IsNullOrWhiteSpace(registerUserModel.Address) ||
                        string.IsNullOrWhiteSpace(registerUserModel.City) ||
                        string.IsNullOrWhiteSpace(registerUserModel.Country) ||
                        string.IsNullOrWhiteSpace(registerUserModel.Username) ||
                        string.IsNullOrWhiteSpace(registerUserModel.Password) ||
                        string.IsNullOrWhiteSpace(registerUserModel.Email)) {
                        return Results.BadRequest();
                    }

                    try {

                        UserModel? _user = await userUtils.GetLoginUserFromCookie(context, db);
                        if (_user != null)
                        {
                            return Results.Redirect("/dishes");
                        }



                        // image => "someimage.png;BASE64=="
                        string? image = registerUserModel.Image;
                        if (image != null && image.Length > 0) {
                            registerUserModel.Image = await controllerUtils.UploadImage(image);
                        }

                        db.RegisterUser(new InsertUserModel(registerUserModel));
                        //db.RegisterUser(registerUserModel);

                        UserModel? user = await db.LoginUser(new UserModel(registerUserModel), 
                            true);
                        if (user == null)
                        {
                            if (registerUserModel.Image != null && 
                                registerUserModel.Image.Length > 0)
                            {
                                controllerUtils.RemoveImage("wwwroot"+ registerUserModel.Image);
                            }
                            return Results.BadRequest();
                        }

                        string authString = userUtils.HandleAuth(user, registerUserModel.RememberMe);

                        context.Response.Cookies.Delete(userUtils.authHeader);
                        context.Response.Cookies.Append(userUtils.authHeader, authString);

                        return Results.Ok();

                } catch (Exception) {
                    return Results.BadRequest();
                }

            }).RequireRateLimiting("fixed")
            .DisableAntiforgery();


            // loggout from profile (by clearing session)
            app.MapPost("/logout", async (HttpContext context) => {
                try {

                    List<string> keys = new List<string>(context.Request.Cookies.Keys);
                    foreach (string key in keys)
                    {
                        context.Response.Cookies.Delete(key);
                    }
                    return Results.Redirect("/login");

                } catch (Exception) {
                    return Results.Unauthorized();
                }

            }).RequireRateLimiting("fixed")
            .DisableAntiforgery();


            //edit user profile
            app.MapPost("/profile/edit", async (UserDatabaseHandler db, HttpContext context,
                ControllerUtils controllerUtils, UserUtils userUtils,
                [FromForm] UpdateUserModel updateUserModel) =>
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(updateUserModel.Address) ||
                        string.IsNullOrWhiteSpace(updateUserModel.City) ||
                        string.IsNullOrWhiteSpace(updateUserModel.Country) ||
                        string.IsNullOrWhiteSpace(updateUserModel.Username))
                    {
                        return Results.BadRequest();
                    }

                    UserModel? model = await userUtils.GetUserModelFromAuth(context);
                    if (model == null)
                    {
                        return Results.Redirect("/login");
                    }

                    string Username = !model.Username.Equals(updateUserModel.Username) ? updateUserModel.Username : model.Username;
                    
                    string Address = !model.Address.Equals(updateUserModel.Address) ? updateUserModel.Address : model.Address;
                    string City = !model.City.Equals(updateUserModel.City) ? updateUserModel.City : model.City;
                    string? State = model.State != null && !model.State.Equals(updateUserModel.State) ? updateUserModel.State : model.State;
                    string Country = !model.Country.Equals(updateUserModel.Country) ? updateUserModel.Country : model.Country;
                    string Email = !model.Email.Equals(updateUserModel.Email) ? updateUserModel.Email : model.Email;

                    string _notes = updateUserModel.Notes?.Replace(" ","").Length == 0 ? null : updateUserModel.Notes;

                    string? Notes = model.Notes != _notes ? _notes : model.Notes;

                    string? Image = null;
                    if (updateUserModel.DeleteImage.Equals("yes") && 
                        model.Image != null && model.Image.Contains('.'))
                    {
                        controllerUtils.RemoveImage("wwwroot"+model.Image);
                        Image = null;

                    } else
                    {
                        updateUserModel.Image = updateUserModel.Image?.Replace(" ", "").Length == 0 ? null : updateUserModel.Image;
                        Image = model.Image;
                        if (updateUserModel.Image != null)
                        {
                            Image = await controllerUtils.UploadImage(updateUserModel.Image);
                        } 
                    }

                    string? _phone = updateUserModel.PhoneNumber?.Replace(" ", "").Length == 0 ? null : updateUserModel.PhoneNumber;
                    string? PhoneNumber = model.PhoneNumber != _phone ? _phone : model.PhoneNumber;

                    UserModel user = new UserModel();
                    user.Address = Address;
                    user.State = State;
                    user.City = City;
                    user.Username = Username;
                    user.Country = Country;
                    user.Notes = Notes;
                    user.Image = Image;
                    user.PhoneNumber = PhoneNumber;
                    user.Email = Email;
                    user.Id = model.Id;

                    db.UpdateUser(user);

                    string authString = userUtils.HandleAuth(user, "off");

                    context.Response.Cookies.Delete(userUtils.authHeader);
                    context.Response.Cookies.Append(userUtils.authHeader, authString);

                    return Results.Ok();

                } catch (Exception)
                {
                    return Results.BadRequest();
                }

            }).RequireRateLimiting("fixed")
            .DisableAntiforgery();
        }
    }
}
