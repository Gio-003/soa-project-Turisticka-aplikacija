package models

import (
	"time"

	"go.mongodb.org/mongo-driver/bson/primitive"
)

type Blog struct {
	ID          primitive.ObjectID `bson:"_id,omitempty" json:"id"`
	Title       string             `bson:"title" json:"title"`
	Description string             `bson:"description" json:"description"`
	CreatedAt   time.Time          `bson:"created_at" json:"created_at"`
	ImageURL    string             `bson:"image_url" json:"image_url"`
	AuthorID    string             `bson:"authorId" json:"authorId"`

	Likes    []string  `bson:"likes" json:"likes"`
	Comments []Comment `bson:"comments,omitempty" json:"comments"`
}
