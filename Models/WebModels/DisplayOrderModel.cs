namespace ITStepFinalProject.Models.WebModels
{
    public class DisplayOrderModel
    {
        public int Id { get; set; }
        public string CurrentStatus { get; set; }
        public string? Notes { get; set; }
        public DateTime OrderedAt { get; set; }
        public decimal TotalPrice { get; set; }

        public int UserId { get; set; }
        public int RestorantId { get; set; }

        public string RestorantAddress { get; set; }
        public string RestorantCity { get; set; }
        public string? RestorantState { get; set; }
        public string RestorantCountry { get; set; }
    }
}
