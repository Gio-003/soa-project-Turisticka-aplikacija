using tour_service.Enum;

namespace tour_service.DTO
{
    public class CreateTourDurationDTO
    {
        public TransportType TransportType { get; set; }

        public int DurationInMinutes { get; set; }
    }
}