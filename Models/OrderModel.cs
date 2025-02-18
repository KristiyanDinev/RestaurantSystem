﻿namespace ITStepFinalProject.Models {
    public class OrderModel {

        public int Id { get; set; }
        public string CurrentStatus { get; set; }
        public string? Notes { get; set; }
        public DateTime OrderedAt { get; set; }
        public float TotalPrice { get; set; }

        public int UserId { get; set; }
        public string FullResturantAddress { get; set; } // address;city;country

    }
}
