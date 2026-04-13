package repo

import (
	"blog-service/models"

	"github.com/google/uuid"
	"gorm.io/gorm"
)

type BlogRepository struct {
	Database *gorm.DB
}

func (r *BlogRepository) CreateBlog(Blog *models.Blog) error {
	return r.Database.Create(Blog).Error
}

func (r *BlogRepository) GetByID(id uuid.UUID) (*models.Blog, error) {
	var blog models.Blog
	err := r.Database.First(&blog, "id = ?", id).Error
	if err != nil {
		return nil, err
	}
	return &blog, nil
}

func (r *BlogRepository) GetAll() ([]models.Blog, error) {
	var blogs []models.Blog
	err := r.Database.Find(&blogs).Error
	return blogs, err
}
