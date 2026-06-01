using tour_service.Enum;

namespace tour_service.DTO
{
    public class TourResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string Difficulty { get; set; }

        public decimal Price { get; set; }
        public TourStatus Status { get; set; }

        public List<string> Tags { get; set; }

        public List<KeyPointResponse> KeyPoints { get; set; }
        public List<TourDurationResponse> Durations { get; set; }

        public double LengthInKm { get; set; }
        public bool IsPurchased { get; set; }
    }
}
