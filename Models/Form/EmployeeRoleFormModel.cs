using RestaurantSystem.Enums;

namespace RestaurantSystem.Models.Form
{
    public class EmployeeRoleFormModel
    {
        public required long Id { get; set; }
        public required RoleEnum Role { get; set; }
    }
}
