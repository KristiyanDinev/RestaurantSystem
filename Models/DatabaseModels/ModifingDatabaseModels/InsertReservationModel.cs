using ITStepFinalProject.Models.Controller;

namespace ITStepFinalProject.Models.DatabaseModels.ModifingDatabaseModels
{
    public class InsertReservationModel
    {
        public string Notes { get; set; }
        public int ReservatorId { get; set; }
        public string CurrentStatus { get; set; }
        public int RestorantId { get; set; }
        public int Amount_Of_Adults { get; set; }
        public int Amount_Of_Children { get; set; }
        public DateTime At_Date { get; set; }

        public InsertReservationModel(RegisterReservationModel registerReservationModel, int userId)
        {
            Notes = registerReservationModel.Notes;
            RestorantId = registerReservationModel.RestorantId;
            Amount_Of_Children = registerReservationModel.Amount_Of_Children;
            Amount_Of_Adults = registerReservationModel.Amount_Of_Adults;
            At_Date = DateTime.ParseExact(registerReservationModel.At_Date, "yyyy-dd-MM HH:mm", null);
            ReservatorId = userId;
        }
    }
}
