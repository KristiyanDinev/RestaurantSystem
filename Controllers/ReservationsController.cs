using RestaurantSystem.Database.Handlers;
using RestaurantSystem.Models.Controller;
using RestaurantSystem.Models.DatabaseModels;
using RestaurantSystem.Models.WebModels;
using RestaurantSystem.Utils.Controller;
using RestaurantSystem.Utils.Web;
using Microsoft.AspNetCore.Mvc;
using RestaurantSystem.Controllers;

namespace RestaurantSystem.Controllers
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
                    UserModel? user = await userUtils.GetUserByJWT(context);
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
                    UserModel? user = await userUtils.GetUserByJWT(context);
                    if (user == null)
                    {
                        return Results.Redirect("/login");
                    }

                    List<TimeTableJoinRestorantModel> timeTableJoinRestorantModels = await reservationDB.GetRestorantsAddressesForReservation(user);

                    string FileData = await controllerUtils.GetHTMLFromWWWROOT("/reservations/create");
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
                    if (registerReservationModel.Amount_Of_Children < 0 ||
                        registerReservationModel.Amount_Of_Adults < 0 ||
                        string.IsNullOrWhiteSpace(registerReservationModel.At_Date))
                    {
                        return Results.BadRequest();
                    }

                    UserModel? user = await userUtils.GetUserByJWT(context);
                    if (user == null)
                    {
                        return Results.Unauthorized();
                    }

                    bool IsCreated = await reservationsDB
                        .CreateReservation(new ReservationModel(registerReservationModel, user.Id),
                        controllerUtils.PendingStatus);

                    if (!IsCreated)
                    {
                        throw new Exception("Limit reached or failed to create it.");
                    }

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
                 WebSocketHandler webSocketHandler,
                [FromForm] string reservationIdStr) =>
            {
                try
                {
                    if (!int.TryParse(reservationIdStr, out int reservationId))
                    {
                        return Results.BadRequest();
                    }

                    UserModel? user = await userUtils.GetUserByJWT(context);
                    if (user == null)
                    {
                        return Results.Unauthorized();
                    }

                    reservationsDB.DeleteReservation(reservationId);
                    webSocketHandler.RemoveModelIdFromReservationSubscribtion(reservationId);

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
