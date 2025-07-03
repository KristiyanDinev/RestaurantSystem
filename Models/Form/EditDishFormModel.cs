namespace RestaurantSystem.Models.Form
{
    public class EditDishFormModel
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string Type { get; set; }
        public required decimal Price { get; set; }
        public required string Ingredients { get; set; }
        public required string AverageTimeToCook { get; set; }
        public required int Grams { get; set; }
        public required bool IsAvailable { get; set; }
        public IFormFile? Image { get; set; }
        public required bool DeleteImage { get; set; } = false;
    }
}
