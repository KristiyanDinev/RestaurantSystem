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
                async (HttpContext context, UserDatabaseHandler db) => {

                    return await ControllerUtils.HandleDefaultPage("/profile",
                        context, db, true);

            }).RequireRateLimiting("fixed");



            // get front-end login page
            app.MapGet("/login", async (HttpContext context) => {
                    try {
                        string data = await ControllerUtils.GetFileContent("/login");

                        return ControllerUtils.IsLoggedIn(context.Session) != null ?

                        Results.Redirect("/dishes") :
                        Results.Content(data, "text/html");

                    } catch (Exception) {
                        return Results.Redirect("/error");
                }

            }).RequireRateLimiting("fixed");


            // try loging in
            app.MapPost("/login", async (UserDatabaseHandler db, HttpContext context,
                [FromForm] string email, [FromForm] string password,
                [FromForm] string rememberMe) => {

                    if (string.IsNullOrWhiteSpace(email) ||
                        string.IsNullOrWhiteSpace(password)) {
                        return Results.BadRequest();
                    }

                    try {
                        ISession session = context.Session;

                        if (ControllerUtils.IsLoggedIn(session) != null) {
                            // user is logged in
                            return Results.Redirect("/dishes");
                        }

                        UserModel user = await db.LoginUser(new UserModel(email, password)) 
                            ?? throw new Exception("Didn't login");

                        ControllerUtils._handleRememberMe(ref session, rememberMe, user.Id);

                        await session.CommitAsync();

                        return Results.Ok();

                    } catch (Exception) {
                        return Results.Unauthorized();
                    }

            }).RequireRateLimiting("fixed")
            .DisableAntiforgery();


            // get front-end register page
            app.MapGet("/register", async (HttpContext context) => {
                try {
                    string data = await ControllerUtils.GetFileContent("/register");

                    return ControllerUtils.IsLoggedIn(context.Session) != null ?

                    Results.Redirect("/dishes") :
                    Results.Content(data, "text/html");

                } catch (Exception) {
                    return Results.Redirect("/error");
                }

            }).RequireRateLimiting("fixed");


            // try registering
            app.MapPost("/register", async (UserDatabaseHandler db, HttpContext context,
                [FromForm] string username, [FromForm] string password, 
                [FromForm] string email, [FromForm] string notes,
                [FromForm] string phone, [FromForm] string fulladdress,
                [FromForm] string image, [FromForm] string rememberMe) => {

                if (string.IsNullOrWhiteSpace(fulladdress) || string.IsNullOrWhiteSpace(email) || 
                    string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password)) {
                        return Results.BadRequest();
                    }

                    try {

                        ISession session = context.Session;

                        if (ControllerUtils.IsLoggedIn(session) != null) {
                            // user is logged in
                            return Results.Redirect("/dishes");
                        }

                        UserModel userModel =
                            ControllerUtils.GetUserModel(username, email, 
                        password, fulladdress, phone, notes);


                        // image => "someimage.png;BASE64=="

                        if (image.Length > 0) {
                            userModel.Image = await ControllerUtils.UploadImage(image);
                        }

                        db.RegisterUser(userModel);
                        UserModel? user = await db.LoginUser(userModel);
                        if (user == null)
                        {
                            if (userModel.Image != null && userModel.Image.Length > 0)
                            {
                                ControllerUtils.RemoveImage("wwwroot"+userModel.Image);
                            }
                            return Results.BadRequest();
                        }
                        
                        ControllerUtils._handleRememberMe(ref session, rememberMe, user.Id);

                        await session.CommitAsync();

                        return Results.Ok();

                } catch (Exception) {
                    return Results.BadRequest();
                }

            }).RequireRateLimiting("fixed")
            .DisableAntiforgery();


            // loggout from profile (by clearing session)
            app.MapPost("/logout", async (HttpContext context) => {
                    try {
                        ISession session = context.Session;
                        session.Clear();
                        await session.CommitAsync();

                        return Results.Redirect("/login");

                    } catch (Exception) {
                        return Results.Unauthorized();
                    }

                }).RequireRateLimiting("fixed")
            .DisableAntiforgery();


            //edit user profile
            app.MapPost("/profile/edit", async (UserDatabaseHandler db, HttpContext context,
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

                    ISession session = context.Session;
                    int? userId = ControllerUtils.IsLoggedIn(session);
                    if (userId == null)
                    {
                        return Results.Unauthorized();
                    }

                    UserModel model = await db.GetUser((int)userId);

                    string Username = model.Username;
                    if (!model.Username.Equals(username))
                    {
                        Username = username;
                    }

                    string FullAddress = model.FullAddress;
                    if (!model.FullAddress.Equals(fulladdress))
                    {
                        FullAddress = fulladdress;
                    }

                    notes = notes?.Replace(" ","").Length == 0 ? null : notes;

                    string? Notes = model.Notes;
                    if (model.Notes != notes)
                    {
                        Notes = notes;
                    }

                    string? Image = null;
                    if (delete_image.Equals("yes") && 
                        model.Image != null && model.Image.Contains('.'))
                    {
                        ControllerUtils.RemoveImage("wwwroot"+model.Image);
                        Image = null;

                    } else
                    {
                        image = image?.Replace(" ", "").Length == 0 ? null : image;
                        Image = model.Image;
                        if (image != null)
                        {
                            Image = await ControllerUtils.UploadImage(image);
                        } 
                    }

                    phone = phone?.Replace(" ", "").Length == 0 ? null : phone;
                    string? PhoneNumber = model.PhoneNumber;
                    if (model.PhoneNumber != phone)
                    {
                        PhoneNumber = phone;
                    }

                    UserModel user = ControllerUtils.GetUserModel(Username, 
                        model.Email, model.Password, FullAddress, PhoneNumber, Notes);

                    user.Id = model.Id;

                    db.UpdateUser(user);

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
