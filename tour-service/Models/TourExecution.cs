using tour_service.Enum;

namespace tour_service.Models
{
    public class TourExecution
    {
        public Guid Id { get; set; }
        public Guid TourId { get; set; }
        public Tour Tour { get; set; }
        public int TouristId { get; set; }
        public ExecutionStatus Status { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? AbandonedAt { get; set; }
        public DateTime LastActivity { get; set; }
        public double StartLatitude { get; set; }
        public double StartLongitude { get; set; }
        public List<CompletedKeyPoint> CompletedKeyPoints { get; set; } = new();
    }
}
