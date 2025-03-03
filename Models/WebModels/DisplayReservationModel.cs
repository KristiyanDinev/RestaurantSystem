namespace ITStepFinalProject.Models.WebModels
{
    public class DisplayReservationModel
    {
        public int Id { get; set; }
        public string Notes { get; set; }
        public int ReservatorId { get; set; }
        public string CurrentStatus { get; set; }
        public int RestorantId { get; set; }
        public int Amount_Of_Adults { get; set; }
        public int Amount_Of_Children { get; set; }
        public DateTime At_Date { get; set; }
        public DateTime Created_At { get; set; }

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
