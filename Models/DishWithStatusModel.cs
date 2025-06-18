using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Models
{
    public class DishWithStatusModel
    {
        public required DishModel Dish { get; set; }
        public required OrderedDishesModel OrderedDish { get; set; }

        public override int GetHashCode()
        {
            return $"{OrderedDish.DishId},{OrderedDish.OrderId},{OrderedDish.CurrentStatus}"
                .GetHashCode();
        }

        public override bool Equals(object? other)
        {
            if (other is not DishWithStatusModel otherModel)
            {
                return false;
            }

            return this.OrderedDish.DishId == otherModel.OrderedDish.DishId &&
                   this.OrderedDish.OrderId == otherModel.OrderedDish.OrderId &&
                   this.OrderedDish.CurrentStatus == otherModel.OrderedDish.CurrentStatus;
        }
    }
}
