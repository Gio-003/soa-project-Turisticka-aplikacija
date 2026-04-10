package service

import (
	"blog-service/dto"
	"blog-service/models"
	"blog-service/repo"

	"github.com/google/uuid"
)

type CommentService struct {
	Repository *repo.CommentRepository
}

func (service *CommentService) CreateComment(CommentDTO *dto.CommentDTO, authorID string) (*models.Comment, error) {
	blogUUID, err := uuid.Parse(CommentDTO.BlogID)
	if err != nil {
		return nil, err
	}
	comment := &models.Comment{
		BlogID:   blogUUID,
		AuthorID: authorID,
		Content:  CommentDTO.Content,
	}
	err = service.Repository.CreateComment(comment)
	if err != nil {
		return nil, err
	}
	return comment, nil
}
