namespace tour_service.Models
{
    public class TourTag
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid TourId { get; set; }
        public Tour Tour { get; set; }
    }
}
