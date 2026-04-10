package repo

import (
	"blog-service/models"

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
