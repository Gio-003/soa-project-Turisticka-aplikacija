namespace tour_service.Models
{
    public class TourTag
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int TourId { get; set; }
        public Tour Tour { get; set; }
    }
}
