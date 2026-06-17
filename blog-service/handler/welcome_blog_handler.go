package handler

import (
	"blog-service/dto"
	"blog-service/service"
	"encoding/json"
	"log"

	saga "github.com/Gio-003/soa-project-Turisticka-aplikacija/common/saga/messaging"
	"github.com/nats-io/nats.go"
)

type WelcomeBlogCommandHandler struct {
	blogService       *service.BlogService
	commandSubscriber saga.Subscriber
	replyPublisher    saga.Publisher
}

func NewWelcomeBlogCommandHandler(
	blogService *service.BlogService,
	publisher saga.Publisher,
	subscriber saga.Subscriber,
) (*WelcomeBlogCommandHandler, error) {

	h := &WelcomeBlogCommandHandler{
		blogService:       blogService,
		replyPublisher:    publisher,
		commandSubscriber: subscriber,
	}

	err := h.commandSubscriber.Subscribe(h.handle)
	if err != nil {
		return nil, err
	}

	log.Println("WelcomeBlog SAGA handler started")

	return h, nil
}

func (h *WelcomeBlogCommandHandler) handle(msg *nats.Msg) {

	var event dto.WelcomeBlogEvent

	if err := json.Unmarshal(msg.Data, &event); err != nil {
		log.Println("Invalid event:", err)
		return
	}

	reply := dto.WelcomeBlogReply{
		UserId: event.UserId,
	}

	createDTO := &dto.CreateBlogDTO{
		Title:       "Welcome " + event.Username,
		Description: "Welcome blog created automatically",
	}

	_, err := h.blogService.CreateBlog(createDTO, event.UserId)

	if err != nil {
		reply.Type = "WELCOME_BLOG_FAILED"
		log.Println("Failed to create welcome blog")
	} else {
		reply.Type = "WELCOME_BLOG_CREATED"
		log.Println("Welcome blog created for:", event.UserId)
	}

	_ = h.replyPublisher.Publish(reply)
}