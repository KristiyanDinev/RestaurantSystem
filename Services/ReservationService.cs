using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Database;
using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Services
{
    public class ReservationService
    {

        private DatabaseContext _databaseContext;

        private RestaurantService _restaurantDatabaseHandler;

        public ReservationService(DatabaseContext databaseContext,
            RestaurantService restaurantDatabaseHandler)
        {
            _databaseContext = databaseContext;
            _restaurantDatabaseHandler = restaurantDatabaseHandler;
        }

        public async Task<bool> CreateReservation(int userId, int restaurantId, 
            int amount_Of_Adults, int amount_Of_Children, DateTime dateTime, string? notes)
        {
            ReservationModel reservation = new ReservationModel();
            reservation.UserId = userId;
            reservation.RestaurantId = restaurantId;
            reservation.Amount_Of_Adults = amount_Of_Adults;
            reservation.Amount_Of_Children = amount_Of_Children;
            reservation.At_Date = dateTime;
            reservation.Notes = notes;

            if (!await _restaurantDatabaseHandler.CheckForReservation(reservation)) {
                return false;
            }

            await _databaseContext.Reservations.AddAsync(reservation);

            int num = await _databaseContext.SaveChangesAsync();

            return num >= 1;
        }

        public async Task<List<ReservationModel>> GetReservationsByUserId(int userId)
        {
            return await _databaseContext.Reservations.Where(res => 
                res.UserId == userId).ToListAsync();
        }

        public async Task DeleteReservation(int reservationId)
        {
            ReservationModel? reservation = await _databaseContext
                .Reservations.FirstOrDefaultAsync(res => res.Id == reservationId);

            if (reservation == null) {
                return;
            }

            _databaseContext.Remove(reservation);

            await _databaseContext.SaveChangesAsync();
        }

        public async Task<List<ReservationModel>> GetReservationsByRestaurantId(int restaurantId)
        {
            return await _databaseContext.Reservations.Where(res =>
                res.RestaurantId == restaurantId).ToListAsync();
        }
    }
}
