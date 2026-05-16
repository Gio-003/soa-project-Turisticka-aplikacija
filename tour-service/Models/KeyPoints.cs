namespace tour_service.Models
{
    public class KeyPoints
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public Guid TourId { get; set; }

        public string ImageUrl { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        

    }
}
