package db

import (
	"blog-service/models"
	"fmt"
	"log"
	"os"

	"gorm.io/driver/postgres"
	"gorm.io/gorm"
)

func InitDB() *gorm.DB {
	dsn := os.Getenv("DB_CONNECTION_STRING")

	if dsn == "" {
		dsn = "host=postgres user=postgres password=root dbname=blog_service port=5432"
		log.Println("DB_CONNECTION_STRING not set, using default local DSN.")
	}

	db, err := gorm.Open(postgres.Open(dsn), &gorm.Config{})
	if err != nil {
		log.Fatalf("Failed to connect to database: %v", err)
		return nil
	}

	fmt.Println("Database connection successful.")
	db.AutoMigrate(&models.Blog{}, &models.Comment{})
	return db
}
