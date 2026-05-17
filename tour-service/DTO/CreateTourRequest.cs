namespace tour_service.DTO
{
    public class CreateTourRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Difficulty { get; set; }

        public List<string> Tags { get; set; }

        public List<KeyPointFromFrontDTO> KeyPoints { get; set; } = new();
    }

    public class KeyPointFromFrontDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; } // Povezuje se sa 'image' iz Angulara
        public double Lat { get; set; }   // Povezuje se sa 'lat' iz Angulara
        public double Lng { get; set; }   // Povezuje se sa 'lng' iz Angulara
    }
}
