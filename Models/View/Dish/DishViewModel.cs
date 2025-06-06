﻿using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Models.View.Dish
{
    public class DishViewModel
    {
        public required UserModel User { get; set; }
        public required RestaurantModel Restaurant { get; set; }
        public required DishModel Dish { get; set; }
    }
}
