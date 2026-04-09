package models

import (
	"time"

	"github.com/google/uuid"
	"gorm.io/gorm"
)

type Blog struct {
	ID          uuid.UUID `gorm:"type:uuid;primary_key;"`
	Title       string    `json:"title"`
	Description string    `json:"description"`
	CreatedAt   time.Time `json:"created_at"`
	ImageURL    []string  `json:"image_url" gorm:"type:jsonb"`
	AuthorID    string    `json:"authorId"`
}

func (blog *Blog) BeforeCreate(tx *gorm.DB) (err error) {
	blog.ID = uuid.New()
	return
}
