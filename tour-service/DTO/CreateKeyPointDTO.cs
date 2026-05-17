namespace tour_service.DTO
{
    public class CreateKeyPointDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }

        public double Longitude { get; set; }
        public double Latitude { get; set; }
    }
}
