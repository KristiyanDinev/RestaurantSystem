using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Models
{
    public class DishWithStatusModel
    {
        public required DishModel Dish { get; set; }
        public required OrderedDishesModel OrderedDish { get; set; }

        public override int GetHashCode()
        {
            return (OrderedDish.DishId, OrderedDish.OrderId, 
                OrderedDish.CurrentStatus, OrderedDish.Notes).GetHashCode();
        }

        public override bool Equals(object? other)
        {
            return this.GetHashCode().Equals(other?.GetHashCode());
        }
    }
}
