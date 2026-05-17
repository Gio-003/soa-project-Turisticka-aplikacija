namespace tour_service.DTO
{
    public class CreateTourRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Difficulty { get; set; }

        public List<string> Tags { get; set; }
    }
}
