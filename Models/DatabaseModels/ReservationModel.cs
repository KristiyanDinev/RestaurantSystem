using ITStepFinalProject.Models.Controller;

namespace ITStepFinalProject.Models.DatabaseModels
{
    public class ReservationModel
    {
        public int Id { get; set; }
        public string Notes { get; set; }
        public int ReservatorId { get; set; }
        public string CurrentStatus { get; set; } = "";
        public int RestorantId { get; set; }
        public int Amount_Of_Adults { get; set; }
        public int Amount_Of_Children { get; set; }
        public DateTime At_Date { get; set; }
        public DateTime Created_At { get; set; } // default

        public decimal Price_Per_Adult { get; set; } // default

        public decimal Price_Per_Children { get; set; } // default

        public ReservationModel(RegisterReservationModel registerReservationModel, int userId)
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
