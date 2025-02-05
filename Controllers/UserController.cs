using ITStepFinalProject.Database;
using ITStepFinalProject.Models;
using ITStepFinalProject.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace ITStepFinalProject.Controllers {
    public class UserController {

        // testing user:
        // email: some@email.com
        // pass: 123

        public UserController(WebApplication app) {

            app.MapPost("/user", async (HttpContext context, DatabaseManager db,
                JWTHandler jwt) => {

                // get the SearchUserId form JWT
                Dictionary<string, object>? claims = await 
                        Utils.Utils.GetRestoratCookieClaims(context, jwt);

                if (claims == null) { 
                    return new UserModel();
                }

                    try {
                        UserModel model = await db.GetUser(
                            int.Parse(Convert.ToString(claims["SearchUserId"])));
                        return model;

                    } catch (Exception) {
                        return new UserModel();
                    }
               
            }).RequireRateLimiting("fixed")
            .DisableAntiforgery();

            app.MapPost("/login", async (DatabaseManager db, HttpContext context, 
                JWTHandler jwt,
                [FromForm] string email, [FromForm] string password,
                [FromForm] string rememberMe) => {

                    if (email.Equals("") || password.Equals("")) {
                        return Results.BadRequest();
                    }

                    Dictionary<string, object>? claims = 
                        await Utils.Utils.GetRestoratCookieClaims(context, jwt);


                    if (claims != null) {
                        return Results.BadRequest();
                    }

                    try {

                        UserModel user = await db.LoginUser(email, password);
                        Utils.Utils._handleRememberMe(ref context, rememberMe, user.Id, jwt);

                            Utils.Utils._handleReadableData(ref context, user);

                        return Results.Ok();

                    } catch (Exception e) {
                        Console.WriteLine(e);
                        return Results.Unauthorized();
                    }

            }).RequireRateLimiting("fixed")
            .DisableAntiforgery();

            app.MapPost("/register", async (DatabaseManager db, HttpContext context,
                JWTHandler jwt,
                [FromForm] string username, [FromForm] string password, 
                [FromForm] string email, [FromForm] string notes,
                [FromForm] string phone, [FromForm] string address,
                [FromForm] string image, [FromForm] string rememberMe) => {

                    if (address.Equals("") || email.Equals("") || username.Equals("") ||
                        password.Equals("")) {
                        return Results.BadRequest();
                    }

                    Dictionary<string, object>? claims =
                        await Utils.Utils.GetRestoratCookieClaims(context, jwt);


                    if (claims != null) {
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
                                await fs.WriteAsync(Utils.Utils.FromStringToUint8Array(byteData));

                                userModel.Image = "/user/images/" + imageName;
                            }
                        }

                        UserModel user = await db.RegisterUser(userModel, password);

                        Utils.Utils._handleRememberMe(ref context, rememberMe, user.Id, jwt);

                        Utils.Utils._handleReadableData(ref context, user);


                        return Results.Ok();


                } catch (Exception) {
                    return Results.BadRequest();
                }

            }).RequireRateLimiting("fixed")
            .DisableAntiforgery();

            app.MapPost("/logout", (DatabaseManager db, HttpContext context) => {

                    try {
                    IResponseCookies responseCookies = context.Response.Cookies;
                    responseCookies.Delete("RestorantCookie");

                    responseCookies.Delete("Username");
                    responseCookies.Delete("Email");
                    responseCookies.Delete("Address");
                    responseCookies.Delete("Notes");
                    responseCookies.Delete("Image");
                    responseCookies.Delete("Phone");

                    return Results.Ok();

                    } catch (Exception) {
                        return Results.Unauthorized();
                    }

                }).RequireRateLimiting("fixed")
            .DisableAntiforgery();
        }


        
    }
}
