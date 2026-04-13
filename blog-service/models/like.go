package models

import (
	"time"

	"github.com/google/uuid"
	"gorm.io/gorm"
)

type Like struct {
	ID        uuid.UUID `gorm:"type:uuid;primary_key;"`
	BlogID    uuid.UUID `gorm:"type:uuid;not null;index:idx_blog_user_like,unique"`
	UserID    string    `gorm:"not null;index:idx_blog_user_like,unique" json:"userId"`
	CreatedAt time.Time `json:"created_at"`
}

func (like *Like) BeforeCreate(tx *gorm.DB) (err error) {
	like.ID = uuid.New()
	like.CreatedAt = time.Now()
	return
}
