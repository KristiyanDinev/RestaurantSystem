namespace ITStepFinalProject.Models
{
    public class OrderJoinDishesModels
    {
        public int OrderId { get; set; }
        public string OrderCurrentStatus { get; set; }
        public string? OrderNotes { get; set; }
        public DateTime OrderOrderedAt { get; set; }
        public float OrderTotalPrice { get; set; }

        public int OrderUserId { get; set; }
        public string OrderFullResturantAddress { get; set; } // address;city;country

        public int DishId { get; set; }
        public string DishName { get; set; }
        public float DishPrice { get; set; }
        public int DishGrams { get; set; }
        public string? DishImage { get; set; }
        public string DishIngredients { get; set; }
        public string DishType_Of_Dish { get; set; }
        public bool DishIsAvailable { get; set; }
        public string DishAvrageTimeToCook { get; set; }
    }
}
