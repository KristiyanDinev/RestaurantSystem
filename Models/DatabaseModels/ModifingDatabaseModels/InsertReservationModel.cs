namespace ITStepFinalProject.Models.DatabaseModels.ModifingDatabaseModels
{
    public class InsertReservationModel
    {
        public int ReservatorId { get; set; }
        public string Notes { get; set; }
        public string CurrentStatus { get; set; }
        public int Amount_Of_Adults { get; set; }
        public int Amount_Of_Children { get; set; }
        public string At_Date { get; set; }
    }
}
