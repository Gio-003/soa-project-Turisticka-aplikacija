namespace tour_service.Models
{
    public class Review
    {
        public Guid Id { get; set; }

        public Guid TourId { get; set; }
        public Tour? Tour { get; set; }

        public int Rating { get; set; } // 1-5
        public string Comment { get; set; } = string.Empty;

        public int TouristId { get; set; }
        public string TouristUsername { get; set; } = string.Empty;

        public DateTime VisitDate { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<string> Images { get; set; } = new(); // List of image URLs
    }
}
