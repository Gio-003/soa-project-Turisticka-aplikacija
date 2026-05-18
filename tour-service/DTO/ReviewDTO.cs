namespace tour_service.DTO
{
    public class CreateReviewRequest
    {
        public int Rating { get; set; } // 1-5
        public string Comment { get; set; } = string.Empty;
        public DateTime VisitDate { get; set; }
        public List<string> Images { get; set; } = new();
    }

    public class ReviewResponse
    {
        public Guid Id { get; set; }
        public Guid TourId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public int TouristId { get; set; }
        public string TouristUsername { get; set; } = string.Empty;
        public DateTime VisitDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string> Images { get; set; } = new();
    }
}
