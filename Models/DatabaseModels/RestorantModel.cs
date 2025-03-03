namespace ITStepFinalProject.Models.DatabaseModels
{
    public class RestorantModel
    {
        public int Id { get; set; }
        public string RestorantAddress { get; set; }
        public string RestorantCity { get; set; }
        public string? RestorantState { get; set; }
        public string RestorantCountry { get; set; }
        public bool DoDelivery { get; set; }
        public bool ServeCustomersInPlace { get; set; }

        public int ReservationMaxChildren { get; set; }
        public int ReservationMinChildren { get; set; }
        public int ReservationMaxAdults { get; set; }
        public int ReservationMinAdults { get; set; }
    }
}
