using Microsoft.AspNetCore.Identity;

namespace RestaurantSystem.Models.DatabaseModels
{
    public class RoleModel : IdentityRole<long>
    {
        public string? Description { get; set; }
    }
}
