using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Database;
using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Services
{
    public class ReservationService
    {

        private DatabaseContext _databaseManager;

        private RestaurantService _restaurantDatabaseHandler;

        public ReservationService(DatabaseContext databaseManager,
            RestaurantService restaurantDatabaseHandler)
        {
            _databaseManager = databaseManager;
            _restaurantDatabaseHandler = restaurantDatabaseHandler;
        }

        public async Task<bool> CreateReservation(int userId, int restaurantId, 
            int amount_Of_Adults, int amount_Of_Children, DateTime dateTime, string? notes)
        {
            ReservationModel reservation = new ReservationModel();
            reservation.UserModelId = userId;
            reservation.RestaurantModelId = restaurantId;
            reservation.Amount_Of_Adults = amount_Of_Adults;
            reservation.Amount_Of_Children = amount_Of_Children;
            reservation.At_Date = dateTime;
            reservation.Notes = notes;

            if (!await _restaurantDatabaseHandler.CheckForReservation(reservation)) {
                return false;
            }

            await _databaseManager.Reservations.AddAsync(reservation);

            int num = await _databaseManager.SaveChangesAsync();

            return num >= 1;
        }

        public async Task<List<ReservationModel>> GetReservationsByUserId(int userId)
        {
            return await _databaseManager.Reservations.Where(res => 
                res.UserModelId == userId)
                .ToListAsync();
        }

        public async Task DeleteReservation(int reservationId)
        {
            ReservationModel? reservation = await _databaseManager
                .Reservations.FirstOrDefaultAsync(res => res.Id == reservationId);

            if (reservation == null) {
                return;
            }

            _databaseManager.Remove(reservation);

            await _databaseManager.SaveChangesAsync();
        }
    }
}
