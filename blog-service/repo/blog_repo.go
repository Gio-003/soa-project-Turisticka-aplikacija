package repo

import (
	"blog-service/models"
	"context"
	"time"

	"go.mongodb.org/mongo-driver/bson"
	"go.mongodb.org/mongo-driver/bson/primitive"
	"go.mongodb.org/mongo-driver/mongo"
)

type BlogRepository struct {
	Cli *mongo.Client
}

func (r *BlogRepository) collection() *mongo.Collection {
	return r.Cli.Database("blogdb").Collection("blogs")
}

func (r *BlogRepository) Create(blog *models.Blog) error {
	ctx, cancel := context.WithTimeout(context.Background(), 5*time.Second)
	defer cancel()

	blog.ID = primitive.NewObjectID()
	blog.CreatedAt = time.Now()
	blog.Likes = []string{}
	blog.Comments = []models.Comment{}

	_, err := r.collection().InsertOne(ctx, blog)
	return err
}

func (r *BlogRepository) GetAll() ([]models.Blog, error) {
	ctx, cancel := context.WithTimeout(context.Background(), 5*time.Second)
	defer cancel()
	cursor, err := r.collection().Find(ctx, bson.M{})
	if err != nil {
		return nil, err
	}
	defer cursor.Close(ctx)

	var blogs []models.Blog
	for cursor.Next(ctx) {
		var blog models.Blog
		if err := cursor.Decode(&blog); err != nil {
			return nil, err
		}
		blogs = append(blogs, blog)
	}
	return blogs, nil
}
func (r *BlogRepository) Like(blogID, userID string) error {
	ctx, cancel := context.WithTimeout(context.Background(), 5*time.Second)
	defer cancel()
	objID, err := primitive.ObjectIDFromHex(blogID)
	if err != nil {
		return err
	}

	update := bson.M{
		"$addToSet": bson.M{
			"likes": userID,
		},
	}
	_, err = r.collection().UpdateOne(ctx, primitive.M{"_id": objID}, update)
	return err
}
func (r *BlogRepository) Unlike(blogID, userID string) error {
	ctx, cancel := context.WithTimeout(context.Background(), 5*time.Second)
	defer cancel()
	objID, err := primitive.ObjectIDFromHex(blogID)
	if err != nil {
		return err
	}

	update := bson.M{
		"$pull": bson.M{
			"likes": userID,
		},
	}
	_, err = r.collection().UpdateOne(ctx, primitive.M{"_id": objID}, update)
	return err
}

func (r *BlogRepository) AddComment(blogID string, comment *models.Comment) error {
	ctx, cancel := context.WithTimeout(context.Background(), 5*time.Second)
	defer cancel()
	objID, err := primitive.ObjectIDFromHex(blogID)
	if err != nil {
		return err
	}
	comment.ID = primitive.NewObjectID()
	comment.CreatedAt = time.Now()
	update := bson.M{
		"$push": bson.M{
			"comments": comment,
		},
	}
	_, err = r.collection().UpdateOne(ctx, bson.M{"_id": objID}, update)
	return err
}

func (r *BlogRepository) GetByID(blogID string) (*models.Blog, error) {
	ctx, cancel := context.WithTimeout(context.Background(), 5*time.Second)
	defer cancel()
	objID, err := primitive.ObjectIDFromHex(blogID)
	if err != nil {
		return nil, err
	}
	var blog models.Blog
	err = r.collection().FindOne(ctx, bson.M{"_id": objID}).Decode(&blog)
	if err != nil {
		return nil, err
	}
	return &blog, nil
}
