using tour_service.Enum;

namespace tour_service.Models
{
    public class Review
    {
        public Guid Id { get; set; }

        public Guid TourId { get; set; }
        public Tour Tour { get; set; }

        public Guid ReviewerUserId { get; set; }

        public ReviewRating Rating { get; set; }

        public string Comment { get; set; }

        public DateTime VisitDate { get; set; }

        public DateTime ReviewDate { get; set; } = DateTime.UtcNow;

        // JSON array of base64 encoded images
        public string Images { get; set; } = "[]";

        // User info snapshot - sačuvano sa recenzijom
        public string ReviewerName { get; set; }
        public string ReviewerProfilePicture { get; set; }
    }
}
