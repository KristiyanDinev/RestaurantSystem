using RestaurantSystem.Models.Controller;
using RestaurantSystem.Models.DatabaseModels;
using RestaurantSystem.Models.WebModels;
using RestaurantSystem.Utils.Web;
using Microsoft.AspNetCore.Mvc;
using RestaurantSystem.Controllers;
using RestaurantSystem.Services;
using RestaurantSystem.Utils;

namespace RestaurantSystem.Controllers
{
    public class ReservationsController
    {
        public ReservationsController(WebApplication app)
        {
            app.MapGet("/Reservations", async (HttpContext context, 
                ControllerUtils controllerUtils, UserUtils userUtils, WebUtils webUtils,
                ReservationService reservationsDB) =>
            {
                try
                {
                    UserModel? user = await userUtils.GetUserByJWT(context);
                    if (user == null)
                    {
                        return Results.Redirect("/login");
                    }

                    List<DisplayReservationModel> reservations = await reservationsDB.GetReservationsByUser(user);

                    string FileData = await controllerUtils.GetHTMLFromWWWROOT("/Reservations");
                    FileData = webUtils.HandleCommonPlaceholders(FileData, controllerUtils.UserModelName, [user]);
                    FileData = webUtils.HandleCommonPlaceholders(FileData, controllerUtils.ReservationModelName, 
                        reservations.Cast<object>().ToList());

                    return Results.Content(FileData, "text/html");

                }
                catch (Exception)
                {
                    return Results.Redirect("/_restaurant_error");
                }
            }).RequireRateLimiting("fixed");

            app.MapGet("/Reservations/create", async (HttpContext context,
                ControllerUtils controllerUtils, UserUtils userUtils, WebUtils webUtils, 
                ReservationService reservationDB) =>
            {
                try
                {
                    UserModel? user = await userUtils.GetUserByJWT(context);
                    if (user == null)
                    {
                        return Results.Redirect("/login");
                    }

                    List<TimeTableJoinRestorantModel> timeTableJoinRestorantModels = await reservationDB.GetRestorantsAddressesForReservation(user);

                    string FileData = await controllerUtils.GetHTMLFromWWWROOT("/Reservations/create");
                    FileData = webUtils.HandleCommonPlaceholders(FileData, controllerUtils.UserModelName, [user]);
                    FileData = webUtils.HandleCommonPlaceholders(FileData, controllerUtils.RestorantModelName,
                        timeTableJoinRestorantModels.Cast<object>().ToList());

                    return Results.Content(FileData, "text/html");

                }
                catch (Exception)
                {
                    return Results.Redirect("/_restaurant_error");
                }
            }).RequireRateLimiting("fixed");


            app.MapPost("/Reservations/create", async (HttpContext context,
                UserUtils userUtils, ReservationService reservationsDB,
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

            app.MapPost("/Reservations/delete", async (HttpContext context,
                UserUtils userUtils, ReservationService reservationsDB,
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
