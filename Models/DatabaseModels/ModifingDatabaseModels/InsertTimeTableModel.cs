namespace ITStepFinalProject.Models.DatabaseModels.ModifingDatabaseModels
{
    public class InsertTimeTableModel
    {
        public int RestorantId { get; set; }
        public string UserAddress { get; set; }
        public string UserCiry { get; set; }
        public string? UserState { get; set; }
        public string UserCountry { get; set; }
        public string AvrageDeliverTime { get; set; }

        public InsertTimeTableModel() { }
    }
}
