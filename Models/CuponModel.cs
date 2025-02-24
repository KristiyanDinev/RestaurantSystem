namespace ITStepFinalProject.Models {
    public class CuponModel {

        public string Name { get; set; }
        public string CuponCode { get; set; }
        public decimal DiscountPercent { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
