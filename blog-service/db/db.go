package db

import (
	"context"
	"log"
	"os"
	"time"

	"go.mongodb.org/mongo-driver/mongo"
	"go.mongodb.org/mongo-driver/mongo/options"
	"go.mongodb.org/mongo-driver/mongo/readpref"
)

type MongoClient struct {
	Client *mongo.Client
}

func InitDB() *MongoClient {
	mongoURI := os.Getenv("MONGO_URI")

	if mongoURI == "" {
		// Fallback na lokalnu instancu ako varijabla nije postavljena
		mongoURI = "mongodb://localhost:27017"
		log.Println("MONGO_URI not set, using default local URI.")
	}

	// Postavljanje opcija za klijenta
	clientOptions := options.Client().ApplyURI(mongoURI)
	ctx, cancel := context.WithTimeout(context.Background(), 10*time.Second)
	defer cancel()

	// Povezivanje na MongoDB
	client, err := mongo.Connect(ctx, clientOptions)
	if err != nil {
		log.Fatalf("Failed to connect to MongoDB: %v", err)
		return nil
	}

	log.Println("Successfully connected to MongoDB!")
	return &MongoClient{Client: client}
}

func (client *MongoClient) Disconnect(ctx context.Context) error {
	err := client.Client.Disconnect(ctx)
	if err != nil {
		log.Fatalf("Failed to disconnect from MongoDB: %v", err)
		return err
	}
	log.Println("Successfully disconnected from MongoDB!")
	return nil
}

func (client *MongoClient) Ping() {
	ctx, cancel := context.WithTimeout(context.Background(), 2*time.Second)
	defer cancel()
	err := client.Client.Ping(ctx, readpref.Primary())
	if err != nil {
		log.Fatalf("Failed to ping MongoDB: %v", err)
	}
	log.Println("Successfully pinged MongoDB!")

}
