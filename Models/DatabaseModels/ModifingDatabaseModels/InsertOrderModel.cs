using ITStepFinalProject.Models.DatabaseModels;

namespace ITStepFinalProject.Models.DatabaseModels.ModifingDatabaseModels
{
    public class InsertOrderModel
    {
        public string CurrentStatus { get; set; }
        public string? Notes { get; set; }
        public decimal TotalPrice { get; set; }

        public int UserId { get; set; }
        public string RestorantAddress { get; set; } // address;city;country
        public string RestorantCity { get; set; }
        public string? RestorantState { get; set; } 
        public string RestorantCountry { get; set; } 

        public InsertOrderModel() { }
        public InsertOrderModel(OrderModel order)
        {
            CurrentStatus = order.CurrentStatus;
            Notes = order.Notes;
            TotalPrice = order.TotalPrice;
            UserId = order.UserId;
            RestorantAddress = order.RestorantAddress;
            RestorantCity = order.RestorantCity;
            RestorantState = order.RestorantState;
            RestorantCountry = order.RestorantCountry;
        }
    }
}
