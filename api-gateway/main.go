package main

import (
	grpcclient "api-gateway/grpc"
	"api-gateway/middleware"
	"api-gateway/router"
	"api-gateway/handlers" 
	"log"
	"net/http"

	"github.com/gorilla/mux"
)

func main() {

	// CORS middleware na svim rutama

	grpcclient.InitBlogClient()
	defer grpcclient.CloseBlogClient()
	muxRouter := mux.NewRouter()
	muxRouter.Use(middleware.CORSMiddleware)

	// Health check
	muxRouter.HandleFunc("/health", healthCheck).Methods("GET", "OPTIONS")

	// Auth rute - BEZ JWT-a
	muxRouter.HandleFunc("/api/auth/login", router.ProxyAuth).Methods("POST", "OPTIONS")
	muxRouter.HandleFunc("/api/auth/signup", router.ProxyAuth).Methods("POST", "OPTIONS")

	//GRPC ZA BLOG SERVICE
	// Blog rute - SA JWT-om
	muxRouter.Handle("/api/blogs", middleware.JWTMiddlewareFunc(router.GetAllBlogsGrpc)).Methods("GET", "OPTIONS")
	muxRouter.Handle("/api/blogs/{id}", middleware.JWTMiddlewareFunc(router.GetBlogByIDGrpc)).Methods("GET", "OPTIONS")

	muxRouter.Handle("/api/blogs", middleware.JWTMiddlewareFunc(router.ProxyBlog)).Methods("POST", "OPTIONS")

	muxRouter.Handle("/api/blogs/{blogId}/likes", middleware.JWTMiddlewareFunc(router.ProxyBlog)).Methods("POST", "DELETE", "OPTIONS")
	muxRouter.Handle("/api/blogs/{blogId}/comments", middleware.JWTMiddlewareFunc(router.ProxyBlog)).Methods("GET", "POST", "OPTIONS")
	muxRouter.Handle("/api/comments", middleware.JWTMiddlewareFunc(router.ProxyBlog)).Methods("POST", "GET", "OPTIONS")
	muxRouter.Handle("/api/comments/{id}", middleware.JWTMiddlewareFunc(router.ProxyBlog)).Methods("PUT", "OPTIONS")

	// Tour rute - SA JWT-om
	muxRouter.Handle("/api/tours/all", middleware.JWTMiddlewareFunc(router.ProxyTour)).Methods("GET", "OPTIONS")
	muxRouter.Handle("/api/tours/my/{authorId}", middleware.JWTMiddlewareFunc(router.ProxyTour)).Methods("GET", "OPTIONS")
	muxRouter.Handle("/api/tours/{tourId}/durations/{durationId}", middleware.JWTMiddlewareFunc(router.ProxyTour)).Methods("PUT", "DELETE", "OPTIONS")
	muxRouter.Handle("/api/tours/{tourId}/durations", middleware.JWTMiddlewareFunc(router.ProxyTour)).Methods("POST", "GET", "OPTIONS")
	muxRouter.Handle("/api/tours/{id}/publish", middleware.JWTMiddlewareFunc(router.ProxyTour)).Methods("POST", "OPTIONS")
	muxRouter.Handle("/api/tours/{id}/archive", middleware.JWTMiddlewareFunc(router.ProxyTour)).Methods("POST", "OPTIONS")
	muxRouter.Handle("/api/tours/{tourId}/length", middleware.JWTMiddlewareFunc(router.ProxyTour)).Methods("PUT", "OPTIONS")
	muxRouter.Handle("/api/tours/all", middleware.JWTMiddlewareFunc(handlers.GetAllToursRPC),).Methods("GET")
	muxRouter.Handle("/api/tours/my/{authorId}", middleware.JWTMiddlewareFunc(handlers.GetMyToursRPC),).Methods("GET")	
	muxRouter.Handle("/api/tours", middleware.JWTMiddlewareFunc(router.ProxyTour)).Methods("POST", "GET", "OPTIONS")
	muxRouter.Handle("/api/tours/{id}", middleware.JWTMiddlewareFunc(router.ProxyTour)).Methods("GET", "OPTIONS")
	muxRouter.Handle("/api/tours/{tourId}/keypoints", middleware.JWTMiddlewareFunc(router.ProxyTour)).Methods("GET", "POST", "OPTIONS")
	muxRouter.Handle("/api/tours/{tourId}/keypoints/{id}", middleware.JWTMiddlewareFunc(router.ProxyTour)).Methods("PUT", "DELETE", "OPTIONS")
	muxRouter.Handle("/api/tours/{tourId}/reviews", middleware.JWTMiddlewareFunc(router.ProxyTour)).Methods("GET", "POST", "OPTIONS")
	muxRouter.Handle("/api/tours/{tourId}/durations", middleware.JWTMiddlewareFunc(router.ProxyTour)).Methods("GET", "POST", "OPTIONS")
	muxRouter.Handle("/api/tours/{tourId}/durations/{id}", middleware.JWTMiddlewareFunc(router.ProxyTour)).Methods("PUT", "DELETE", "OPTIONS")
	muxRouter.Handle("/api/tours/{id}/publish",middleware.JWTMiddlewareFunc(router.ProxyTour),).Methods("POST", "OPTIONS")
	muxRouter.Handle("/api/tours/{id}/archive",middleware.JWTMiddlewareFunc(router.ProxyTour),).Methods("POST", "OPTIONS")

	// Follower rute - SA JWT-om
	muxRouter.Handle("/api/followers/{followerId}/follow/{followedId}", middleware.JWTMiddlewareFunc(router.ProxyFollower)).Methods("POST", "DELETE", "OPTIONS")
	muxRouter.Handle("/api/followers/{followerId}/following", middleware.JWTMiddlewareFunc(router.ProxyFollower)).Methods("GET", "OPTIONS")
	muxRouter.Handle("/api/followers/{followerId}/followers", middleware.JWTMiddlewareFunc(router.ProxyFollower)).Methods("GET", "OPTIONS")
	muxRouter.Handle("/api/followers/{followerId}/is-following/{followedId}", middleware.JWTMiddlewareFunc(router.ProxyFollower)).Methods("GET", "OPTIONS")
	muxRouter.Handle("/api/followers/{userId}/recommendations", middleware.JWTMiddlewareFunc(router.ProxyFollower)).Methods("GET", "OPTIONS")
	muxRouter.Handle("/api/followers/{userId}/can-read/{authorId}", middleware.JWTMiddlewareFunc(router.ProxyFollower)).Methods("GET", "OPTIONS")
	muxRouter.Handle("/api/followers/{userId}/can-comment/{authorId}", middleware.JWTMiddlewareFunc(router.ProxyFollower)).Methods("GET", "OPTIONS")

	// User/Profile rute - SA JWT-om
	muxRouter.Handle("/api/getMyInfo", middleware.JWTMiddlewareFunc(router.ProxyAuth)).Methods("GET", "OPTIONS")
	muxRouter.Handle("/api/me", middleware.JWTMiddlewareFunc(router.ProxyAuth)).Methods("GET", "PUT", "OPTIONS")
	muxRouter.Handle("/api/updateMyInfo", middleware.JWTMiddlewareFunc(router.ProxyAuth)).Methods("PUT", "OPTIONS")
	muxRouter.Handle("/api/user/all", middleware.JWTMiddlewareFunc(router.ProxyAuth)).Methods("GET", "OPTIONS")

	// Admin rute - SA JWT-om + Admin provjerom
	muxRouter.Handle("/api/admin/users", middleware.JWTMiddlewareFunc(middleware.AdminMiddlewareFunc(router.ProxyAuth))).Methods("GET", "OPTIONS")
	muxRouter.Handle("/api/admin/users/{id}/block", middleware.JWTMiddlewareFunc(middleware.AdminMiddlewareFunc(router.ProxyAuth))).Methods("PUT", "OPTIONS")
	muxRouter.Handle("/api/user/all", middleware.JWTMiddlewareFunc(middleware.AdminMiddlewareFunc(router.ProxyAuth))).Methods("GET", "OPTIONS")

	port := ":8000"
	log.Println("🚀 API Gateway starting on port" + port)

	if err := http.ListenAndServe(port, muxRouter); err != nil {
		log.Fatal("Failed to start gateway: ", err)
	}
}

func healthCheck(w http.ResponseWriter, r *http.Request) {
	w.Header().Set("Content-Type", "application/json")
	w.WriteHeader(http.StatusOK)
	w.Write([]byte(`{"status":"healthy","gateway":"api-gateway","version":"1.0"}`))
}
