namespace ITStepFinalProject.Models.DatabaseModels
{
    public class ReservationModel
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
    }
}
