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
	likeRepository := &repo.LikeRepository{Database: dbConn}
	blogService := &service.BlogService{Repository: blogRepository, LikeRepository: likeRepository}
	blogHandler := &handler.BlogHandler{Service: blogService}
	likeService := &service.LikeService{LikeRepository: likeRepository, BlogRepository: blogRepository}
	likeHandler := &handler.LikeHandler{Service: likeService}

	commentRepository := &repo.CommentRepository{Database: dbConn}
	commentService := &service.CommentService{Repository: commentRepository}
	commentHandler := &handler.CommentHandler{Service: commentService}

	router := mux.NewRouter()
	router.HandleFunc("/blogs", blogHandler.CreateBlog).Methods("POST")
	router.HandleFunc("/blogs", blogHandler.GetAllBlogs).Methods("GET")
	router.HandleFunc("/blogs/{id}", blogHandler.GetBlogByID).Methods("GET")
	router.HandleFunc("/blogs/{blogId}/likes", likeHandler.LikeBlog).Methods("POST")
	router.HandleFunc("/blogs/{blogId}/likes", likeHandler.UnlikeBlog).Methods("DELETE")
	router.HandleFunc("/comments", commentHandler.CreateComment).Methods("POST")
	router.HandleFunc("/blogs/{blogId}/comments", commentHandler.GetCommentsByBlogID).Methods("GET")
	router.HandleFunc("/comments", commentHandler.GetAllComments).Methods("GET")
	router.HandleFunc("/comments/{id}", commentHandler.UpdateComment).Methods("PUT")

	port := ":8080"
	log.Println("Starting server on " + port)

	if err := http.ListenAndServe(port, router); err != nil {
		log.Fatal("Failed to start server: ", err)
	}
}
