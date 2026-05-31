using tour_service.Enum;

namespace tour_service.Models
{
    public class Tour
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public string Difficulty { get; set; } // može i enum ako želiš

        public List<TourTag> Tags { get; set; } = new();

        public TourStatus Status { get; set; }

        public decimal Price { get; set; }

        public int AuthorId { get; set; }

        public List<KeyPoints> KeyPoints { get; set; } = new();

        public List<TourDuration> Durations { get; set; } = new();
        public double LengthInKm { get; set; }

        public DateTime? PublishedAt { get; set; }

        public DateTime? ArchivedAt { get; set; }
    }
}
