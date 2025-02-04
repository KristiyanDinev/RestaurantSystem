namespace ITStepFinalProject.Models {
    public class DishModel {

        public int Id { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }
        public int Grams { get; set; }
        public string? Image { get; set; }
        public string Type_Of_Dish { get; set; }
        public bool IsAvailable { get; set; }
        public string AvrageTimeToCook { get; set; }

    }
}
