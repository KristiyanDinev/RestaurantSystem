namespace ITStepFinalProject.Models.DatabaseModels
{
    public class TimeTableJoinRestorantModel
    {
        public int RestorantId { get; set; }
        public string RestorantAddress { get; set; }
        public string RestorantCity { get; set; }
        public string? RestorantState { get; set; }
        public string RestorantCountry { get; set; }
        public bool DoDelivery { get; set; }
        public bool ServeCustomersInPlace { get; set; }

        public string UserAddress { get; set; }
        public string UserCity { get; set; }
        public string? UserState { get; set; }
        public string UserCountry { get; set; }
        public string AvrageDeliverTime { get; set; }
    }
}
