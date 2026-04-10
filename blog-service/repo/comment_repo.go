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
