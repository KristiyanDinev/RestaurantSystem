
namespace ITStepFinalProject.Models.DatabaseModels.ModifingDatabaseModels
{
    public class InsertOrderModel
    {
        public string CurrentStatus { get; set; }
        public string? Notes { get; set; }
        public decimal TotalPrice { get; set; }

        public int UserId { get; set; }
        public int RestorantId { get; set; }

        public InsertOrderModel() { }
        public InsertOrderModel(OrderModel order)
        {
            CurrentStatus = order.CurrentStatus;
            Notes = order.Notes;
            TotalPrice = order.TotalPrice;
            UserId = order.UserId;
            RestorantId = order.RestorantId;
        }
    }
}
