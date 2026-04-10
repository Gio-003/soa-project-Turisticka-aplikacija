package repo

import (
	"blog-service/models"

	"github.com/google/uuid"
	"gorm.io/gorm"
)

type CommentRepository struct {
	Database *gorm.DB
}

func (r *CommentRepository) CreateComment(Comment *models.Comment) error {
	return r.Database.Create(Comment).Error
}
func (r *CommentRepository) GetCommentsByBlogID(blogID string) ([]models.Comment, error) {
	var comments []models.Comment
	err := r.Database.Where("blog_id = ?", blogID).Find(&comments).Error
	return comments, err
}
func (r *CommentRepository) GettAllComments() ([]models.Comment, error) {
	var comments []models.Comment
	err := r.Database.Find(&comments).Error
	return comments, err
}
func (r *CommentRepository) GetById(id uuid.UUID) (*models.Comment, error) {
	var comment models.Comment
	err := r.Database.First(&comment, "id = ?", id).Error
	if err != nil {
		return nil, err
	}
	return &comment, err
}
func (r *CommentRepository) UpdateComment(comment *models.Comment) error {
	return r.Database.Save(comment).Error
}
