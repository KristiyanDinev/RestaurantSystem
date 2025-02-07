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
                            string[] imageParts = image.Split(';');
                            string imageName = imageParts[0];
                            string imageData = imageParts[1];

                            string byteData = Encoding.UTF8.GetString(
                                Convert.FromBase64String(imageData));
                            if (!(imageName.Contains('\\') ||
                                imageName.Contains('/') ||
                                imageName.Contains('\'') ||
                                byteData.EndsWith(',') ||
                                byteData.StartsWith(','))) {
                                string imgPath = "wwwroot/images/user/" + imageName;
                                using FileStream fs = 
                                    File.Create("wwwroot/images/user/" + imageName);
                                await fs.WriteAsync(Utils.Utils.FromStringToUint8Array(byteData));

                                userModel.Image = "/images/user/" + imageName;
                            }
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
        }
    }
}
