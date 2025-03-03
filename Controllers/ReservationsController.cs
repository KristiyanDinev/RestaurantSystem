using ITStepFinalProject.Database.Handlers;
using ITStepFinalProject.Models.Controller;
using ITStepFinalProject.Models.DatabaseModels;
using ITStepFinalProject.Models.DatabaseModels.ModifingDatabaseModels;
using ITStepFinalProject.Models.WebModels;
using ITStepFinalProject.Utils.Controller;
using ITStepFinalProject.Utils.Web;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace ITStepFinalProject.Controllers
{
    public class ReservationsController
    {
        public ReservationsController(WebApplication app)
        {
            app.MapGet("/reservations", async (HttpContext context, 
                ControllerUtils controllerUtils, UserUtils userUtils, WebUtils webUtils,
                ReservationDatabaseHandler reservationsDB) =>
            {
                try
                {
                    UserModel? user = await userUtils.GetUserModelFromAuth(context);
                    if (user == null)
                    {
                        return Results.Redirect("/login");
                    }

                    List<DisplayReservationModel> reservations = await reservationsDB.GetReservationsByUser(user);

                    string FileData = await controllerUtils.GetHTMLFromWWWROOT("/reservations");
                    FileData = webUtils.HandleCommonPlaceholders(FileData, controllerUtils.UserModelName, [user]);
                    FileData = webUtils.HandleCommonPlaceholders(FileData, controllerUtils.ReservationModelName, 
                        reservations.Cast<object>().ToList());

                    return Results.Content(FileData, "text/html");

                }
                catch (Exception)
                {
                    return Results.Redirect("/error");
                }
            }).RequireRateLimiting("fixed");

            app.MapGet("/reservations/create", async (HttpContext context,
                ControllerUtils controllerUtils, UserUtils userUtils, WebUtils webUtils, 
                ReservationDatabaseHandler reservationDB) =>
            {
                try
                {
                    UserModel? user = await userUtils.GetUserModelFromAuth(context);
                    if (user == null)
                    {
                        return Results.Redirect("/login");
                    }

                    List<TimeTableJoinRestorantModel> timeTableJoinRestorantModels = await reservationDB.GetRestorantsAddressesForReservation(user);

                    string FileData = await controllerUtils.GetHTMLFromWWWROOT("/reservations");
                    FileData = webUtils.HandleCommonPlaceholders(FileData, controllerUtils.UserModelName, [user]);
                    FileData = webUtils.HandleCommonPlaceholders(FileData, controllerUtils.RestorantModelName,
                        timeTableJoinRestorantModels.Cast<object>().ToList());

                    return Results.Content(FileData, "text/html");

                }
                catch (Exception)
                {
                    return Results.Redirect("/error");
                }
            }).RequireRateLimiting("fixed");


            app.MapPost("/reservations/create", async (HttpContext context,
                UserUtils userUtils, ReservationDatabaseHandler reservationsDB,
                ControllerUtils controllerUtils,
                [FromForm] RegisterReservationModel registerReservationModel) =>
            {
                try
                {
                    UserModel? user = await userUtils.GetUserModelFromAuth(context);
                    if (user == null)
                    {
                        return Results.Unauthorized();
                    }

                    reservationsDB.CreateReservation(new InsertReservationModel(registerReservationModel, 
                        user.Id),
                        controllerUtils.PendingStatus);

                    return Results.Ok();

                }
                catch (Exception)
                {
                    return Results.BadRequest();
                }

            }).DisableAntiforgery()
            .RequireRateLimiting("fixed");

            app.MapPost("/reservations/delete", async (HttpContext context,
                UserUtils userUtils, ReservationDatabaseHandler reservationsDB,
                [FromForm] string reservationIdStr) =>
            {
                try
                {
                    if (!int.TryParse(reservationIdStr, out int reservationId))
                    {
                        return Results.BadRequest();
                    }

                    UserModel? user = await userUtils.GetUserModelFromAuth(context);
                    if (user == null)
                    {
                        return Results.Unauthorized();
                    }

                    reservationsDB.DeleteReservation(reservationId);

                    return Results.Ok();

                }
                catch (Exception)
                {
                    return Results.BadRequest();
                }

            }).DisableAntiforgery()
            .RequireRateLimiting("fixed");
        }
    }
}
