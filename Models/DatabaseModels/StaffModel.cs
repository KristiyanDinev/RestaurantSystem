namespace ITStepFinalProject.Models.DatabaseModels
{
    public class StaffModel
    {
        public int UserId { set; get; }
        public int RestorantId { set; get; }
        public bool IsManager { set; get; }

        public StaffModel() { }

        public StaffModel(int userId, int restorantId, bool isManager)
        {
            UserId = userId;
            RestorantId = restorantId;
            IsManager = isManager;
        }
    }
}
