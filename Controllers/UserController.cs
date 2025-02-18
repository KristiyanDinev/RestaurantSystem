using ITStepFinalProject.Database;
using ITStepFinalProject.Database.Models;
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



            // get front-end searched user profile (full page)
            app.MapGet("/profile/{SearchUserIdStr}",
                async (HttpContext context, UserDatabaseHandler db, 
                string SearchUserIdStr) => {

                    int? id = Utils.ControllerUtils.IsLoggedIn(context.Session);
                    if (id == null) {
                        return Results.Redirect("/login");
                    }

                    if (SearchUserIdStr.Contains('.')) {
                        string a = await Utils.ControllerUtils.GetFileContent("/profile/" + SearchUserIdStr);
                        return Results.Content(a);
                    }

                    try {
                        string FileData = await Utils.ControllerUtils.GetFileContent(context.Request.Path);
                        if (!int.TryParse(SearchUserIdStr, out int SearchUserId)) {

                            Utils.ControllerUtils._handleEntryInFile(ref FileData, new UserModel(), "User");

                            return Results.Content(FileData, "text/html");
                        }

                        UserModel model = await db.GetUser(SearchUserId);

                        Utils.ControllerUtils._handleEntryInFile(ref FileData, model, "User");

                        UserModel user = await db.GetUser((int)id);
                        //Utils.Utils.ApplyUserBarElement(ref FileData, user);

                        return Results.Content(FileData, "text/html");


                    } catch (Exception) {
                        return Results.Redirect("/error");
                    }
            }).RequireRateLimiting("fixed");


            // get front-end logged user profile (full page)
            app.MapGet("/profile",
                async (HttpContext context, UserDatabaseHandler db) => {

                    int? userId = Utils.ControllerUtils.IsLoggedIn(context.Session);
                    if (userId == null) {
                        return Results.Redirect("/login");
                    }

                    try {

                        string FileData = await Utils.ControllerUtils.GetFileContent("/profile");

                        UserModel model = await db.GetUser((int)userId);

                        Utils.ControllerUtils._handleEntryInFile(ref FileData, model, "User");

                        return Results.Content(FileData, "text/html");


                    } catch (Exception) {
                        return Results.Redirect("/error");
                    }
                }).RequireRateLimiting("fixed");


            // get front-end login page
            app.MapGet("/login", async (HttpContext context) => {

                    try {

                        string data = await Utils.ControllerUtils.GetFileContent("/login");


                        return Utils.ControllerUtils.IsLoggedIn(context.Session) != null ?

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

                    if (email.Equals("") || password.Equals("")) {
                        return Results.BadRequest();
                    }

                    try {
                        ISession session = context.Session;

                        if (Utils.ControllerUtils.IsLoggedIn(session) != null) {
                            // user is logged in
                            return Results.Redirect("/dishes");
                        }


                        UserModel user = await db.LoginUser(new UserModel(email, password)) ?? throw new Exception("Didn't login");

                        Utils.ControllerUtils._handleRememberMe(ref session, rememberMe, user.Id);

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

                    string data = await Utils.ControllerUtils.GetFileContent("/register");

                    return Utils.ControllerUtils.IsLoggedIn(context.Session) != null ?

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

                    if (fulladdress.Length == 0 || email.Length == 0 || username.Length == 0 ||
                        password.Length == 0) {
                        return Results.BadRequest();
                    }

                    try {

                        ISession session = context.Session;

                        if (Utils.ControllerUtils.IsLoggedIn(session) != null) {
                            // user is logged in
                            return Results.Redirect("/dishes");
                        }

                        UserModel userModel =
                        Utils.ControllerUtils.GetUserModel(username, email, 
                        password, fulladdress, phone, notes);


                        // image => "someimage.png;BASE64=="

                        if (image.Length > 0) {
                            userModel.Image = await Utils.ControllerUtils.UploadImage(image);
                        }

                        db.RegisterUser(userModel);
                        UserModel? user = await db.LoginUser(userModel);
                        if (user == null)
                        {
                            if (userModel.Image != null && userModel.Image.Length > 0)
                            {
                                Utils.ControllerUtils.RemoveImage("wwwroot"+userModel.Image);
                            }
                            return Results.BadRequest();
                        }
                        
                        Utils.ControllerUtils._handleRememberMe(ref session, rememberMe, user.Id);

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
                    if (username.Length == 0 || fulladdress.Length == 0) {
                        return Results.BadRequest();
                    }

                    ISession session = context.Session;
                    int? userId = Utils.ControllerUtils.IsLoggedIn(session);
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
                        Utils.ControllerUtils.RemoveImage("wwwroot"+model.Image);
                        Image = null;

                    } else
                    {
                        image = image?.Replace(" ", "").Length == 0 ? null : image;
                        Image = model.Image;
                        if (image != null)
                        {
                            Image = await Utils.ControllerUtils.UploadImage(image);
                        }
                    }

                    phone = phone?.Replace(" ", "").Length == 0 ? null : phone;
                    string? PhoneNumber = model.PhoneNumber;
                    if (model.PhoneNumber != phone)
                    {
                        PhoneNumber = phone;
                    }

                    UserModel user = Utils.ControllerUtils.GetUserModel(Username, 
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
