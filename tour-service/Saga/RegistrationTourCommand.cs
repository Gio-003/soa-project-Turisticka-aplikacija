namespace tour_service.Saga
{
    public class RegistrationTourCommand
    {
        public string UserId { get; set; } = "";
        public string Username { get; set; } = "";
        public RegistrationCommandType Type { get; set; }
    }

    public enum RegistrationCommandType
    {
        CreateDraftTour
    }

    public class RegistrationTourReply
    {
        public string UserId { get; set; } = "";
        public string TourId { get; set; } = "";
        public RegistrationReplyType Type { get; set; }
    }

    public enum RegistrationReplyType
    {
        DraftTourCreated,
        DraftTourFailed
    }
}