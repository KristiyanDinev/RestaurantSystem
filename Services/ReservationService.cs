using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Database;
using RestaurantSystem.Enums;
using RestaurantSystem.Models.DatabaseModels;
using RestaurantSystem.Utilities;

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

        public async Task<ReservationModel?> CreateReservationAsync(long userId, int restaurantId,
            int amount_Of_Adults, int amount_Of_Children, DateTime dateTime, string? notes)
        {
            ReservationModel reservation = new ReservationModel()
            {
                UserId = userId,
                RestaurantId = restaurantId,
                Amount_Of_Adults = amount_Of_Adults,
                Amount_Of_Children = amount_Of_Children,
                At_Date = dateTime.ToUniversalTime(),
                Notes = notes,
                CurrentStatus = Status.Pending.ToString()
            };

            if (!await _restaurantDatabaseHandler.CheckForReservationAsync(reservation))
            {
                return null;
            }

            await _databaseContext.Reservations.AddAsync(reservation);

            return await _databaseContext.SaveChangesAsync() > 0 ? reservation : null;
        }

        public async Task<List<ReservationModel>> GetReservationsByUserIdAsync(long userId)
        {
            return await _databaseContext.Reservations.Where(res =>
                res.UserId == userId).ToListAsync();
        }

        public async Task<bool> DeleteReservationAsync(int reservationId)
        {
            ReservationModel? reservation = await _databaseContext
                .Reservations.FirstOrDefaultAsync(res => res.Id == reservationId);

            if (reservation == null)
            {
                return false;
            }

            _databaseContext.Remove(reservation);

            return await _databaseContext.SaveChangesAsync() > 0;
        }

        public async Task<List<ReservationModel>> GetReservationsByRestaurantIdAsync(
            int restaurantId)
        {
            return await _databaseContext.Reservations
                .Include(reservation => reservation.User)
                .Where(res =>
                res.RestaurantId == restaurantId).ToListAsync();
        }


        public async Task<bool> UpdateReservationAsync(int id, string new_status)
        {
            ReservationModel? existingReservation = await _databaseContext
                .Reservations.FirstOrDefaultAsync(res => res.Id == id);
            if (existingReservation == null)
            {
                return false;
            }

            existingReservation.CurrentStatus = Utility.MakeCapital(new_status);
            return await _databaseContext.SaveChangesAsync() > 0;
        }

        public async Task<ReservationModel?> GetReservationByIdAsync(int id)
        {
            return await _databaseContext.Reservations
                .FirstOrDefaultAsync(res => res.Id == id);
        }
    }
}
