using tour_service.Enum;

namespace tour_service.Models
{
    public class TourDuration
    {
        public Guid Id { get; set; }

        public Guid TourId { get; set; }

        public Tour Tour { get; set; }

        public TransportType TransportType { get; set; }

        public int DurationInMinutes { get; set; }
    }
}