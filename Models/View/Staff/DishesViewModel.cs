﻿using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Models.View.Staff
{
    public class DishesViewModel
    {
        public required UserModel Staff { get; set; }

        public List<OrderWithDishesCountModel> OrderWithDishesCount { get; set; } = 
            new List<OrderWithDishesCountModel>();
    }
}
