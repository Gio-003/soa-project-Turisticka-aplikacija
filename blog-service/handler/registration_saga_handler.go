package handler

import (
	"blog-service/dto"
	"blog-service/service"
	"encoding/json"
	"log"

	"github.com/nats-io/nats.go"
)

type RegistrationSagaHandler struct {
	blogService *service.BlogService
	conn        *nats.Conn
}

func NewRegistrationSagaHandler(
	blogService *service.BlogService,
	conn *nats.Conn,
) (*RegistrationSagaHandler, error) {

	h := &RegistrationSagaHandler{
		blogService: blogService,
		conn:        conn,
	}

	_, err := conn.QueueSubscribe("registration.blog.command", "blog-service-group", h.handle)
	if err != nil {
		return nil, err
	}

	log.Println("Registration SAGA handler started")
	return h, nil
}

func (h *RegistrationSagaHandler) handle(msg *nats.Msg) {
	var command struct {
		UserId   string `json:"userId"`
		Username string `json:"username"`
		BlogId   string `json:"blogId"`
		Type     int    `json:"type"`
	}

	if err := json.Unmarshal(msg.Data, &command); err != nil {
		log.Println("Invalid registration command:", err)
		return
	}

	switch command.Type {
	case 0: // CreateWelcomeBlog
		createDTO := &dto.CreateBlogDTO{
			Title:       "Welcome, " + command.Username + "!",
			Description: "This is your welcome blog. Start writing!",
		}
		blog, err := h.blogService.CreateBlog(createDTO, command.UserId)

		var reply map[string]interface{}
		if err != nil {
			log.Printf("Failed to create welcome blog for user %s: %v", command.UserId, err)
			reply = map[string]interface{}{
				"userId": command.UserId,
				"type":   1, // WelcomeBlogFailed
			}
		} else {
			log.Printf("Welcome blog created: %s for user %s", blog.ID.Hex(), command.UserId)
			reply = map[string]interface{}{
				"userId": command.UserId,
				"blogId": blog.ID.Hex(),
				"type":   0, // WelcomeBlogCreated
			}
		}

		if msg.Reply != "" {
			replyBytes, _ := json.Marshal(reply)
			if err := h.conn.Publish(msg.Reply, replyBytes); err != nil {
				log.Println("Failed to send reply:", err)
			}
		}

	case 2: // RollbackBlog
		err := h.blogService.DeleteBlog(command.BlogId)
		if err != nil {
			log.Printf("Rollback failed — could not delete blog %s: %v", command.BlogId, err)
		} else {
			log.Printf("Rollback OK — blog %s deleted", command.BlogId)
		}
		// kompenzacija ne šalje reply
	}
}