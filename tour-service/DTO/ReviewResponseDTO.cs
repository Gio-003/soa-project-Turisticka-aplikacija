using tour_service.Enum;

namespace tour_service.DTO
{
    public class ReviewResponseDTO
    {
        public Guid Id { get; set; }
        public Guid TourId { get; set; }
        public Guid ReviewerUserId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime VisitDate { get; set; }
        public DateTime ReviewDate { get; set; }
        public List<string> Images { get; set; }
        public string ReviewerName { get; set; }
        public string ReviewerProfilePicture { get; set; }
    }
}
