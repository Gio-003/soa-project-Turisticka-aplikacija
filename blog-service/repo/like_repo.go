package repo

import (
	"blog-service/models"

	"github.com/google/uuid"
	"gorm.io/gorm"
)

type LikeRepository struct {
	Database *gorm.DB
}

func (r *LikeRepository) CreateLike(like *models.Like) error {
	return r.Database.Create(like).Error
}

func (r *LikeRepository) GetByBlogAndUser(blogID uuid.UUID, userID string) (*models.Like, error) {
	var like models.Like
	err := r.Database.Where("blog_id = ? AND user_id = ?", blogID, userID).First(&like).Error
	if err != nil {
		return nil, err
	}
	return &like, nil
}

func (r *LikeRepository) DeleteLikeByID(id uuid.UUID) error {
	return r.Database.Delete(&models.Like{}, "id = ?", id).Error
}

func (r *LikeRepository) CountByBlogID(blogID uuid.UUID) (int64, error) {
	var count int64
	err := r.Database.Model(&models.Like{}).Where("blog_id = ?", blogID).Count(&count).Error
	return count, err
}
