using ITStepFinalProject.Database;
using ITStepFinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;

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
                async (HttpContext context, DatabaseManager db, 
                string SearchUserIdStr) => {

                    if (Utils.Utils.IsLoggedIn(context.Session) == null) {
                        return Results.Redirect("/login");
                    }

                    if (SearchUserIdStr.Contains('.')) {
                        string a = await Utils.Utils.GetFileContent("/profile/" + SearchUserIdStr);
                        return Results.Content(a);
                    }

                    try {
                        string FileData = await Utils.Utils.GetFileContent(context.Request.Path);
                        if (!int.TryParse(SearchUserIdStr, out int SearchUserId)) {

                            Utils.Utils._handleEmptyEntryInFile(ref FileData, new UserModel());

                            return Results.Content(FileData, "text/html");
                        }

                        UserModel model = await db.GetUser(SearchUserId);

                        Utils.Utils._handleEntryInFile(ref FileData, model);

                        return Results.Content(FileData, "text/html");


                    } catch (Exception) {
                        return Results.Redirect("/error");
                    }
            }).RequireRateLimiting("fixed");


            // get front-end logged user profile (full page)
            app.MapGet("/profile",
                async (HttpContext context, DatabaseManager db) => {

                    int? userId = Utils.Utils.IsLoggedIn(context.Session);
                    if (userId == null) {
                        return Results.Redirect("/login");
                    }

                    try {

                        string FileData = await Utils.Utils.GetFileContent("/profile");

                        UserModel model = await db.GetUser((int)userId);

                        Utils.Utils._handleEntryInFile(ref FileData, model);

                        return Results.Content(FileData, "text/html");


                    } catch (Exception) {
                        return Results.Redirect("/error");
                    }
                }).RequireRateLimiting("fixed");


            // get front-end login page
            app.MapGet("/login", async (DatabaseManager db, HttpContext context) => {

                    try {

                        string data = await Utils.Utils.GetFileContent("/login");


                        return Utils.Utils.IsLoggedIn(context.Session) != null ?

                        Results.Redirect("/dishes") :
                        Results.Content(data, "text/html");

                    } catch (Exception) {
                        return Results.Redirect("/error");
                }

            }).RequireRateLimiting("fixed");


            // try loging in
            app.MapPost("/login", async (DatabaseManager db, HttpContext context,
                [FromForm] string email, [FromForm] string password,
                [FromForm] string rememberMe) => {

                    if (email.Equals("") || password.Equals("")) {
                        return Results.BadRequest();
                    }

                    try {
                        ISession session = context.Session;

                        if (Utils.Utils.IsLoggedIn(session) != null) {
                            throw new Exception();
                        }

                        UserModel user = await db.LoginUser(email, password);
                        Utils.Utils._handleRememberMe(ref session, rememberMe, user.Id);


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

                    string data = await Utils.Utils.GetFileContent("/register");

                    return Utils.Utils.IsLoggedIn(context.Session) != null ?

                    Results.Redirect("/dishes") :
                    Results.Content(data, "text/html");

                } catch (Exception) {
                    return Results.Redirect("/error");
                }

            }).RequireRateLimiting("fixed");


            // try registering
            app.MapPost("/register", async (DatabaseManager db, HttpContext context,
                [FromForm] string username, [FromForm] string password, 
                [FromForm] string email, [FromForm] string notes,
                [FromForm] string phone, [FromForm] string address,
                [FromForm] string image, [FromForm] string rememberMe) => {

                    if (address.Equals("") || email.Equals("") || username.Equals("") ||
                        password.Equals("")) {
                        return Results.BadRequest();
                    }


                    try {

                        ISession session = context.Session;

                        if (Utils.Utils.IsLoggedIn(session) != null) {
                            throw new Exception();
                        }

                        UserModel userModel = new UserModel();
                        userModel.Username = username;
                        userModel.Email = email;
                        userModel.Address = address;
                        userModel.PhoneNumber = phone;
                        userModel.Notes = notes;


                        // image => "someimage.png;BASE64=="

                        if (image.Length > 0) {
                            userModel.Image = await Utils.Utils.UploadImage(image);
                        }

                        UserModel user = await db.RegisterUser(userModel, password);
                        
                        Utils.Utils._handleRememberMe(ref session, rememberMe, user.Id);

                        await session.CommitAsync();

                        return Results.Ok();


                } catch (Exception) {
                    return Results.BadRequest();
                }

            }).RequireRateLimiting("fixed")
            .DisableAntiforgery();


            // loggout from profile (by clearing session)
            app.MapPost("/logout", async (DatabaseManager db, HttpContext context) => {

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
            app.MapPost("/profile/edit", async (DatabaseManager db, HttpContext context,
                [FromForm] string username, 
                [FromForm] string email, [FromForm] string? notes,
                [FromForm] string? phone, [FromForm] string address,
                [FromForm] string? image, [FromForm] string delete_image) =>
            {


                

                try
                {
                    if (username.Length == 0 || address.Length == 0 || email.Length == 0) {
                        return Results.BadRequest();
                    }

                    ISession session = context.Session;
                    int? userId = Utils.Utils.IsLoggedIn(session);
                    if (userId == null)
                    {
                        return Results.Unauthorized();
                    }

                    int id = (int)userId;

                    UserModel model = await db.GetUser(id);

                    UserModel user = new UserModel();
                    user.Id = (int) userId;

                    user.Username = model.Username;
                    if (!model.Username.Equals(username))
                    {
                        user.Username = username;
                    }

                    user.Email = model.Email;
                    if (!model.Email.Equals(email))
                    {
                        user.Email = email;
                    }

                    user.Address = model.Address;
                    if (!model.Address.Equals(address))
                    {
                        user.Address = address;
                    }

                    notes = notes?.Replace(" ","").Length == 0 ? null : notes;

                    user.Notes = model.Notes;
                    if (model.Notes != notes)
                    {
                        user.Notes = notes;
                    }

                    phone = phone?.Replace(" ", "").Length == 0 ? null : phone;

                    user.PhoneNumber = model.PhoneNumber;
                    if (model.PhoneNumber != phone)
                    {
                        user.PhoneNumber = phone;
                    }

                    if (delete_image.Equals("yes") && 
                    model.Image != null && model.Image.Contains('.'))
                    {
                        Utils.Utils.RemoveImage("wwwroot"+model.Image);
                        user.Image = null;

                    } else
                    {
                        image = image?.Replace(" ", "").Length == 0 ? null : image;
                        user.Image = model.Image;
                        if (image != null)
                        {
                            user.Image = await Utils.Utils.UploadImage(image);
                        }
                    }


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
