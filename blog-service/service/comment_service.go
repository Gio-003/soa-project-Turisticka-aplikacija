package service

import "blog-service/repo"

// import (
// 	"blog-service/dto"
// 	"blog-service/models"
// 	"blog-service/repo"
// 	"time"

// 	"github.com/google/uuid"
// )

type CommentService struct {
	Repository *repo.BlogRepository
}

// func (service *CommentService) CreateComment(CommentDTO *dto.CommentDTO, authorID string) (*models.Comment, error) {
// 	blogUUID, err := uuid.Parse(CommentDTO.BlogID)
// 	if err != nil {
// 		return nil, err
// 	}
// 	comment := &models.Comment{
// 		BlogID:   blogUUID,
// 		AuthorID: authorID,
// 		Content:  CommentDTO.Content,
// 	}
// 	err = service.Repository.CreateComment(comment)
// 	if err != nil {
// 		return nil, err
// 	}
// 	return comment, nil
// }
// func (service *CommentService) GetCommentsByBlogID(blogID string) ([]models.Comment, error) {
// 	return service.Repository.GetCommentsByBlogID(blogID)
// }
// func (service *CommentService) GetAllComments() ([]models.Comment, error) {
// 	return service.Repository.GettAllComments()
// }

// func (service *CommentService) UpdateComment(commentID string, updateDTO *dto.UpdateCommentDTO) (*models.Comment, error) {
// 	commentUUID, err := uuid.Parse(commentID)
// 	if err != nil {
// 		return nil, err
// 	}
// 	comment, err := service.Repository.GetById(commentUUID)
// 	if err != nil {
// 		return nil, err
// 	}
// 	comment.Content = updateDTO.Content
// 	comment.UpdatedAt = time.Now()
// 	err = service.Repository.UpdateComment(comment)
// 	if err != nil {
// 		return nil, err
// 	}
// 	return comment, nil

// }
