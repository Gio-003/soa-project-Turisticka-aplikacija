using tour_service.Enum;

namespace tour_service.DTO
{
    public class CreateReviewDTO
    {
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime VisitDate { get; set; }
        public List<string> Images { get; set; } = new(); // Base64 encoded images
    }
}
