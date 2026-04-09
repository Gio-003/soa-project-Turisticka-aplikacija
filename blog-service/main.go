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

	router := mux.NewRouter()
	port := ":8080"
	router.HandleFunc("/blogs", blogHandler.CreateBlog).Methods("POST")
	log.Println("Starting server on " + port)
	if err := http.ListenAndServe(port, router); err != nil {
		log.Fatal("Failed to start server:", err)
	}
	log.Println("Server started on " + port)
	err := http.ListenAndServe(port, router)
	if err != nil {
		log.Fatal("Failed to start server: ", err)
	}
}
