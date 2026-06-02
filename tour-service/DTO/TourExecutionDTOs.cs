namespace tour_service.DTO
{
    public class StartTourExecutionRequest
    {
        public Guid TourId { get; set; }
        public int TouristId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class CheckPositionRequest
    {
        public Guid TourExecutionId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class CheckPositionResponse
    {
        public Guid? NearbyKeyPointId { get; set; }
        public string? NearbyKeyPointName { get; set; }
        public bool KeyPointCompleted { get; set; }
        public DateTime LastActivity { get; set; }
    }

    public class CompletedKeyPointDto
    {
        public Guid KeyPointId { get; set; }
        public DateTime CompletedAt { get; set; }
    }

    public class TourExecutionResponse
    {
        public Guid Id { get; set; }
        public Guid TourId { get; set; }
        public int TouristId { get; set; }
        public string Status { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? AbandonedAt { get; set; }
        public DateTime LastActivity { get; set; }
        public List<CompletedKeyPointDto> CompletedKeyPoints { get; set; } = new();
    }
}
