package main

import (
	"blog-service/db"
	"log"
	"net/http"

	"github.com/gorilla/mux"

	"blog-service/handler"
	"blog-service/repo"
	"blog-service/service"
)

func main() {
	dbConn := db.InitDB()
	if dbConn == nil {
		log.Fatal("Failed to connect to the database. Shutting down.")
	}
	log.Println("Database connected successfully.")

	blogRepository := &repo.BlogRepository{Database: dbConn}
	blogService := &service.BlogService{Repository: blogRepository}
	blogHandler := &handler.BlogHandler{Service: blogService}

	commentRepository := &repo.CommentRepository{Database: dbConn}
	commentService := &service.CommentService{Repository: commentRepository}
	commentHandler := &handler.CommentHandler{Service: commentService}

	router := mux.NewRouter()
	router.HandleFunc("/blogs", blogHandler.CreateBlog).Methods("POST")
	router.HandleFunc("/comments", commentHandler.CreateComment).Methods("POST")

	port := ":8080"
	log.Println("Starting server on " + port)

	if err := http.ListenAndServe(port, router); err != nil {
		log.Fatal("Failed to start server: ", err)
	}
}
