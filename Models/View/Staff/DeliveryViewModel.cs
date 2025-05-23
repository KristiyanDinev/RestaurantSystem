﻿using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Models.View.Admin
{
    public class DeliveryViewModel
    {

        public string? Error { get; set; }

        public UserModel? Staff { get; set; }

        public Dictionary<RestaurantModel, List<OrderModel>>? Orders { get; set; }
    }
}
