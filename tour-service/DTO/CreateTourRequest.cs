using tour_service.Enum;

namespace tour_service.DTO
{
    public class CreateTourRequest
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Difficulty { get; set; }

        public List<string> Tags { get; set; }

        public List<KeyPointFromFrontDTO> KeyPoints { get; set; } = new();

        // NOVO
        public List<TourDurationDTO> Durations { get; set; } = new();

        public int AuthorId { get; set; }

           public double LengthInKm { get; set; }
    }

    public class KeyPointFromFrontDTO
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Image { get; set; }

        public double Lat { get; set; }

        public double Lng { get; set; }
    }

    // NOVO
    public class TourDurationDTO
    {
        public TransportType TransportType { get; set; }

        public int DurationInMinutes { get; set; }
    }
}