namespace RestaurantSystem.Models.DatabaseModels {
    public class DishModel {

        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Grams { get; set; }
        public string? Image { get; set; }
        public string Ingredients { get; set; }
        public string Type_Of_Dish { get; set; }
        public bool IsAvailable { get; set; }
        public string AvrageTimeToCook { get; set; }
        public string? Notes { get; set; }



        public int RestaurantId { get; set; }
        public RestaurantModel Restaurant { get; set; }


        // navigation
        public ICollection<OrderedDishesModel> OrderedDishes { get; set; }
    }
}
