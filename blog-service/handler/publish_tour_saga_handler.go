package handler

import (
	"blog-service/dto"
	"blog-service/service"

	saga "github.com/Gio-003/soa-project-Turisticka-aplikacija/common/saga/messaging"
	events "github.com/Gio-003/soa-project-Turisticka-aplikacija/common/saga/publish_tour"
)

type PublishTourCommandHandler struct {
	blogService       *service.BlogService
	replyPublisher    saga.Publisher
	commandSubscriber saga.Subscriber
}

func NewPublishTourCommandHandler(blogService *service.BlogService, publisher saga.Publisher, subscriber saga.Subscriber) (*PublishTourCommandHandler, error) {
	o := &PublishTourCommandHandler{
		blogService:       blogService,
		replyPublisher:    publisher,
		commandSubscriber: subscriber,
	}
	// Pretplata na komande koje šalje C# orkestrator
	err := o.commandSubscriber.Subscribe(o.handle)
	if err != nil {
		return nil, err
	}
	return o, nil
}

func (handler *PublishTourCommandHandler) handle(command *events.PublishTourCommand) {
	reply := events.PublishTourReply{TourId: command.TourId}

	switch command.Type {
	case events.CreateBlog:
		// Mapiranje podataka iz komande u tvoj DTO
		createBlogDto := &dto.CreateBlogDTO{
			Title:       "New Tour: " + command.Title,
			Description: command.Description,
			ImageURL:    command.ImageUrl,
		}

		// Pozivanje tvoje postojeće logike
		_, err := handler.blogService.CreateBlog(createBlogDto, command.AuthorId)

		if err != nil {
			reply.Type = events.BlogCreationFailed
		} else {
			reply.Type = events.BlogCreated
		}

	default:
		return // Ignorišemo nepoznate komande
	}

	// Slanje odgovora nazad orkestratoru (C# servisu)
	_ = handler.replyPublisher.Publish(reply)
}
