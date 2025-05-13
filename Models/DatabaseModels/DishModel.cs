namespace RestaurantSystem.Models.DatabaseModels {
    public class DishModel {

        public int Id { get; set; }
        public required string Name { get; set; }
        public required decimal Price { get; set; }
        public required int Grams { get; set; }
        public string? Image { get; set; }
        public required string Ingredients { get; set; }
        public required string Type_Of_Dish { get; set; }
        public required bool IsAvailable { get; set; }
        public required string AvrageTimeToCook { get; set; }
        public string? Notes { get; set; }



        public required int RestaurantId { get; set; }
        public RestaurantModel Restaurant { get; set; }


        // navigation
        public ICollection<OrderedDishesModel> OrderedDishes { get; set; }
    }
}
