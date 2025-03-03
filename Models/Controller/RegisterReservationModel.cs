namespace ITStepFinalProject.Models.Controller
{
    public class RegisterReservationModel
    {
        public string Notes { get; set; }
        public int Amount_Of_Children { get; set; }
        public int Amount_Of_Adults { get; set; }
        public int RestorantId { get; set; }
        public string At_Date { get; set; }
    }
}
