package publish_tour

type PublishTourCommandType int8

const (
	CreateBlog PublishTourCommandType = iota
	RollbackTour
)

type PublishTourCommand struct {
	TourId      string                 `json:"tourId"`
	AuthorId    string                 `json:"authorId"`
	Title       string                 `json:"title"`
	Description string                 `json:"description"`
	ImageUrl    string                 `json:"imageUrl"`
	Type        PublishTourCommandType `json:"type"`
}

type PublishTourReplyType int8

const (
	BlogCreated PublishTourReplyType = iota
	BlogCreationFailed
)

type PublishTourReply struct {
	TourId string               `json:"tourId"`
	Type   PublishTourReplyType `json:"type"`
}
