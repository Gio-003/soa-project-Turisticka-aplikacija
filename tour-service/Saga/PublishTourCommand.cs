namespace tour_service.Saga
{
    public class PublishTourCommand
    {
        public string TourId { get; set; }
        public string AuthorId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public PublishTourCommandType Type { get; set; }
    }

    public enum PublishTourCommandType { CreateBlog, RollbackTour, ApprovePublish }

    public class PublishTourReply
    {
        public string TourId { get; set; }
        public PublishTourReplyType Type { get; set; }
    }

    public enum PublishTourReplyType { BlogCreated, BlogCreationFailed, TourRolledBack }
}