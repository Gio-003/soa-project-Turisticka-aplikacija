using tour_service.Enum;
namespace tour_service.DTO
{
    public class TourResponse
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string Difficulty { get; set; }

        public decimal Price { get; set; }
        public TourStatus Status { get; set; }

        public int AuthorId { get; set; }

        public List<string> Tags { get; set; }
    }
}
