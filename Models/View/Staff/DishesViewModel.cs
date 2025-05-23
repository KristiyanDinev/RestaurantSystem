﻿using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Models.View.Admin
{
    public class DishesViewModel
    {
        public required RestaurantModel Restaurant { get; set; }
        public required UserModel Staff { get; set; }

        public List<OrderWithDishesCountModel> OrderWithDishesCount { get; set; } = 
            new List<OrderWithDishesCountModel>();
    }
}
