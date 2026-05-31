package main

import (
	"blog-service/db"
	"blog-service/handler"
	"blog-service/repo"
	"blog-service/service"
	"log"
	"net"
	"net/http"
	"os"

	"google.golang.org/grpc"

	grpcserver "blog-service/grpc"
	pb "blog-service/proto"

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
	grpcBlogServer := &grpcserver.BlogGrpcServer{
		Service: blogService,
	}

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

	lis, err := net.Listen("tcp", ":50051")
	if err != nil {
		log.Fatal("Failed to listen on grpc port: ", err)
	}

	grpcSrv := grpc.NewServer()

	pb.RegisterBlogServiceServer(
		grpcSrv,
		grpcBlogServer,
	)

	go func() {
		log.Println("gRPC server started on :50051")

		if err := grpcSrv.Serve(lis); err != nil {
			log.Fatal("Failed to start grpc server: ", err)
		}
	}()

	port := ":" + os.Getenv("PORT")
	if port == ":" {
		port = ":8081"
	}
	log.Println("Starting server on " + port)

	if err := http.ListenAndServe(port, router); err != nil {
		log.Fatal("Failed to start server: ", err)
	}
}
