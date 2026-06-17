package registration

// ---- KOMANDE ----

type RegistrationCommandType int8

const (
    CreateWelcomeBlog RegistrationCommandType = iota
    CreateDraftTour
    RollbackBlog   // kompenzacija: obriši blog
)

type RegistrationCommand struct {
    UserId   string                  `json:"userId"`
    Username string                  `json:"username"`
    BlogId   string                  `json:"blogId,omitempty"` // popunjava se u kompenzaciji
    Type     RegistrationCommandType `json:"type"`
}

// ---- REPLY-evi ----

type RegistrationReplyType int8

const (
    WelcomeBlogCreated RegistrationReplyType = iota
    WelcomeBlogFailed
    DraftTourCreated
    DraftTourFailed
)

type RegistrationReply struct {
    UserId string                `json:"userId"`
    BlogId string                `json:"blogId,omitempty"`
    TourId string                `json:"tourId,omitempty"`
    Type   RegistrationReplyType `json:"type"`
}