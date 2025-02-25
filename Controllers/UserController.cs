using ITStepFinalProject.Database.Handlers;
using ITStepFinalProject.Utils;
using ITStepFinalProject.Models;
using Microsoft.AspNetCore.Mvc;

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
                ControllerUtils controllerUtils) => {

                    return await controllerUtils.HandleDefaultPage_WithUserModel("/profile",
                        context);

            }).RequireRateLimiting("fixed");



            // get front-end login page
            app.MapGet("/login", async (HttpContext context, ControllerUtils controllerUtils) => {
                    try {
                        UserModel? user = await controllerUtils.GetUserModelFromAuth(context);
                        string data = await controllerUtils.GetFileContent("/login");

                    return user == null ? Results.Content(data, "text/html") :
                        Results.Redirect("/dishes");

                    } catch (Exception) {
                        return Results.Redirect("/error");
                }

            }).RequireRateLimiting("fixed");


            // try loging in
            app.MapPost("/login", async (UserDatabaseHandler db, HttpContext context,
                ControllerUtils controllerUtils,
                [FromForm] string email, [FromForm] string password,
                [FromForm] string rememberMe) => {

                    if (string.IsNullOrWhiteSpace(email) ||
                        string.IsNullOrWhiteSpace(password)) {
                        return Results.BadRequest();
                    }

                    try {
                        UserModel? _user = await controllerUtils.GetUserModelFromAuth(context);
                        if (_user != null)
                        {
                            return Results.Redirect("/dishes");
                        }

                        UserModel user = new UserModel(email, password);
                        user = await db.LoginUser(user, true) 
                            ?? throw new Exception("Didn't login");

                        string authString = controllerUtils.HandleAuth(user, rememberMe);

                        context.Response.Cookies.Delete("Auth");
                        context.Response.Cookies.Append("Auth", authString);

                        return Results.Ok();

                    } catch (Exception) {
                        return Results.Unauthorized();
                    }

            }).RequireRateLimiting("fixed")
            .DisableAntiforgery();


            // get front-end register page
            app.MapGet("/register", async (HttpContext context, ControllerUtils controllerUtils) => {
                try {
                    UserModel? user = await controllerUtils.GetUserModelFromAuth(context);
                    string data = await controllerUtils.GetFileContent("/register");

                    return user == null ? Results.Content(data, "text/html") :
                        Results.Redirect("/dishes");

                } catch (Exception) {
                    return Results.Redirect("/error");
                }

            }).RequireRateLimiting("fixed");


            // try registering
            app.MapPost("/register", async (UserDatabaseHandler db, HttpContext context,
                ControllerUtils controllerUtils,
                [FromForm] string username, [FromForm] string password, 
                [FromForm] string email, [FromForm] string notes,
                [FromForm] string phone, [FromForm] string fulladdress,
                [FromForm] string image, [FromForm] string rememberMe) => {

                if (string.IsNullOrWhiteSpace(fulladdress) || string.IsNullOrWhiteSpace(email) || 
                    string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password)) {
                        return Results.BadRequest();
                    }

                    try {

                        UserModel? user = await controllerUtils.GetUserModelFromAuth(context);
                        if (user != null)
                        {
                            return Results.Redirect("/dishes");
                        }

                        UserModel userModel = new UserModel(fulladdress, phone, 
                            username, notes, email, password);


                        // image => "someimage.png;BASE64=="

                        if (image.Length > 0) {
                            userModel.Image = await controllerUtils.UploadImage(image);
                        }

                        db.RegisterUser(new InsertUserModel(userModel));

                        UserModel? user = await db.LoginUser(userModel, true);
                        if (user == null)
                        {
                            if (userModel.Image != null && userModel.Image.Length > 0)
                            {
                                controllerUtils.RemoveImage("wwwroot"+userModel.Image);
                            }
                            return Results.BadRequest();
                        }

                        string authString = controllerUtils.HandleAuth(user, rememberMe);

                        context.Response.Cookies.Delete("Auth");
                        context.Response.Cookies.Append("Auth", authString);

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
                ControllerUtils controllerUtils,
                [FromForm] string username, [FromForm] string? notes,
                [FromForm] string? phone, [FromForm] string fulladdress,
                [FromForm] string? image, [FromForm] string delete_image) =>
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(username) || 
                        string.IsNullOrWhiteSpace(fulladdress)) {
                        return Results.BadRequest();
                    }

                    UserModel? model = await controllerUtils.GetUserModelFromAuth(context);
                    if (model == null)
                    {
                        return Results.Redirect("/login");
                    }

                    string Username = !model.Username.Equals(username) ? username : model.Username;
                    
                    string FullAddress = !model.FullAddress.Equals(fulladdress) ? fulladdress : model.FullAddress;

                    notes = notes?.Replace(" ","").Length == 0 ? null : notes;

                    string? Notes = model.Notes != notes ? notes : model.Notes;

                    string? Image = null;
                    if (delete_image.Equals("yes") && 
                        model.Image != null && model.Image.Contains('.'))
                    {
                        controllerUtils.RemoveImage("wwwroot"+model.Image);
                        Image = null;

                    } else
                    {
                        image = image?.Replace(" ", "").Length == 0 ? null : image;
                        Image = model.Image;
                        if (image != null)
                        {
                            Image = await controllerUtils.UploadImage(image);
                        } 
                    }

                    phone = phone?.Replace(" ", "").Length == 0 ? null : phone;
                    string? PhoneNumber = model.PhoneNumber != phone ? phone : model.PhoneNumber;

                    UserModel user = new UserModel(FullAddress, PhoneNumber, Username, Notes, 
                        model.Email, model.Password);

                    user.Id = model.Id;

                    db.UpdateUser(user);

                    string authString = controllerUtils.HandleAuth(user, "off");

                    context.Response.Cookies.Delete("Auth");
                    context.Response.Cookies.Append("Auth", authString);

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
