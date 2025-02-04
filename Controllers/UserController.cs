using ITStepFinalProject.Database;
using ITStepFinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ITStepFinalProject.Controllers {
    public class UserController {

        public UserController(WebApplication app) {

            app.MapPost("/user", async ([FromForm] int Id, DatabaseManager db) => {

                UserModel model = await db.GetUser(Id);
                return model;

            }).RequireRateLimiting("fixed")
            .DisableAntiforgery();

            app.MapPost("/login", async (DatabaseManager db, HttpContext context,
                [FromForm] string email, [FromForm] string password,
                [FromForm] string rememberMe) => {

                    try {


                        int Id = await db.LoginUser(email, password);
                        _handleRememberMe(ref context, rememberMe, Id);

                        return Results.Ok();

                    } catch (Exception) {
                        return Results.Unauthorized();
                    }

            }).RequireRateLimiting("fixed")
            .DisableAntiforgery();

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
                                string imgPath = "wwwroot/user/images/" + imageName;
                                using FileStream fs = 
                                    File.Create("wwwroot/user/images/" + imageName);
                                await fs.WriteAsync(FromStringToUint8Array(byteData));

                                userModel.Image = "/user/images/" + imageName;
                            }
                        }

                        int Id = await db.RegisterUser(userModel, password);

                        _handleRememberMe(ref context, rememberMe, Id);
                        

                        return Results.Ok();


                } catch (Exception) {
                    return Results.BadRequest();
                }

            }).RequireRateLimiting("fixed")
            .DisableAntiforgery();


        }


        private static byte[] FromStringToUint8Array(string data) {
            string[] dataNumbers = data.Split(",");
            byte[] byteArray = new byte[dataNumbers.Length];
            for (int i = 0; i < dataNumbers.Length; i++) {
                byteArray[i] = Convert.ToByte(dataNumbers[i]);
            }
            return byteArray;
        }

        private static void _handleRememberMe(ref HttpContext context, 
            string remeberMe, int Id) {
            if (remeberMe.Equals("on")) {
                context.Response.Cookies.Append("UserId", Id.ToString());
            }
        }
    }
}
