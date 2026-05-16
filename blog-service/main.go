package main

import (
	"blog-service/db"
	"blog-service/handler"
	"blog-service/repo"
	"blog-service/service"
	"log"
	"net/http"
	"os"

	"github.com/gorilla/mux"
)

func main() {
	mongoClient := db.InitDB()
	if mongoClient == nil {
		log.Fatal("Failed to connect to the database. Shutting down.")
	}
	log.Println("Database connected successfully.")

	mongoClient.Ping()
	// Inicijalizacija repozitorijuma sa MongoDB klijentom
	blogRepository := &repo.BlogRepository{Cli: mongoClient.Client}

	// Inicijalizacija jedinstvenog servisa
	blogService := &service.BlogService{Repository: blogRepository}

	// Inicijalizacija jedinstvenog handlera
	blogHandler := &handler.BlogHandler{Service: blogService}

	// Definisanje ruta - sve koriste blogHandler
	router := mux.NewRouter()
	router.HandleFunc("/blogs", blogHandler.CreateBlog).Methods("POST")
	router.HandleFunc("/blogs", blogHandler.GetAllBlogs).Methods("GET")
	router.HandleFunc("/blogs/{id}", blogHandler.GetBlogByID).Methods("GET")

	// Rute za lajkove sada koriste blogHandler
	router.HandleFunc("/blogs/{blogId}/likes", blogHandler.LikeBlog).Methods("POST")
	router.HandleFunc("/blogs/{blogId}/likes", blogHandler.UnlikeBlog).Methods("DELETE")

	// Ruta za dodavanje komentara sada koristi blogHandler
	router.HandleFunc("/blogs/{blogId}/comments", blogHandler.AddComment).Methods("POST")

	port := ":" + os.Getenv("PORT")
	if port == ":" {
		port = ":8080"
	}
	log.Println("Starting server on " + port)

	if err := http.ListenAndServe(port, router); err != nil {
		log.Fatal("Failed to start server: ", err)
	}
}
