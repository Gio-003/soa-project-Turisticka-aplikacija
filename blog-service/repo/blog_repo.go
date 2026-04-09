package repo

import (
	"blog-service/models"

	"gorm.io/gorm"
)

type BlogRepository struct {
	Database *gorm.DB
}

func (r *BlogRepository) CreateBlog(Blog *models.Blog) error {
	return r.Database.Create(Blog).Error
}
