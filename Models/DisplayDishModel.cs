namespace ITStepFinalProject.Models
{
    public class DisplayDishModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Grams { get; set; }
        public string? Image { get; set; }
        public string Ingredients { get; set; }
        public string Type_Of_Dish { get; set; }
        public bool IsAvailable { get; set; }
        public string AvrageTimeToCook { get; set; }
        public int Amount { get; set; }

        public DisplayDishModel() { }

        public DisplayDishModel(DishModel model) {
            Id = model.Id;
            Name = model.Name;
            Price = model.Price;
            Grams = model.Grams;
            Image = model.Image;
            Ingredients = model.Ingredients;
            Type_Of_Dish = model.Type_Of_Dish;
            IsAvailable = model.IsAvailable;
            AvrageTimeToCook = model.AvrageTimeToCook;
            Amount = 0;
        }
    }
}
