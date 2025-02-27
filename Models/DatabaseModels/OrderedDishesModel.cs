namespace ITStepFinalProject.Models.DatabaseModels
{
    public class OrderedDishesModel
    {
        public int OrderId { get; set; }
        public int DishId { get; set; }

        public OrderedDishesModel() { }
        public OrderedDishesModel(int orderId, int dishId)
        {
            OrderId = orderId;
            DishId = dishId;
        }
    }
}
