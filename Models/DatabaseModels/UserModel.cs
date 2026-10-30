using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantSystem.Models.DatabaseModels {
    public class UserModel : IdentityUser<long> {
        public required string Name { get; set; }
        public string? Image { get; set; }
        public DateOnly CreatedAt { get; set; }
        public DateOnly LastTimeLoggedIn { get; set; }


        public int? RestaurantId { get; set; }
        public RestaurantModel? Restaurant { get; set; }



        [NotMapped]
        public ICollection<RoleModel> Roles { get; set; } = new List<RoleModel>();
        public ICollection<OrderModel> Orders { get; set; } = new List<OrderModel>();
        public ICollection<ReservationModel> Reservations { get; set; } = new List<ReservationModel>();
        public ICollection<DeliveryModel> Deliveries { get; set; } = new List<DeliveryModel>();
        public ICollection<AddressModel> Addresses { get; set; } = new List<AddressModel>();

    }
}
