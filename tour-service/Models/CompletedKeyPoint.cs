namespace tour_service.Models
{
    public class CompletedKeyPoint
    {
        public Guid Id { get; set; }
        public Guid TourExecutionId { get; set; }
        public TourExecution TourExecution { get; set; }
        public Guid KeyPointId { get; set; }
        public DateTime CompletedAt { get; set; }
    }
}
