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

        public Guid AuthorId { get; set; }

        public List<KeyPoints> KeyPoints { get; set; } = new();
    }
}
