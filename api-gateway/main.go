package main

import (
	"api-gateway/middleware"
	"api-gateway/router"
	"log"
	"net/http"
	"strings"

	"github.com/gorilla/mux"
)

func main() {
	muxRouter := mux.NewRouter()

	// CORS middleware
	muxRouter.Use(middleware.CORSMiddleware)

	// Health check (no auth needed)
	muxRouter.HandleFunc("/health", healthCheck).Methods("GET")

	// Auth routes (no JWT required) - MUST be registered before protected routes
	authRouter := muxRouter.PathPrefix("/api/auth").Subrouter()
	authRouter.HandleFunc("/login", router.ProxyAuth).Methods("POST")
	authRouter.HandleFunc("/signup", router.ProxyAuth).Methods("POST")

	// Protected routes (JWT required) - exclude /api/auth routes
	protectedRouter := muxRouter.NewRoute().PathPrefix("/api").MatcherFunc(func(r *http.Request, rm *mux.RouteMatch) bool {
		return !strings.HasPrefix(r.URL.Path, "/api/auth")
	}).Subrouter()
	protectedRouter.Use(middleware.JWTMiddleware)

	// Blog routes
	protectedRouter.HandleFunc("/blogs", router.ProxyBlog).Methods("POST", "GET")
	protectedRouter.HandleFunc("/blogs/{id}", router.ProxyBlog).Methods("GET")
	protectedRouter.HandleFunc("/blogs/{blogId}/likes", router.ProxyBlog).Methods("POST", "DELETE")
	protectedRouter.HandleFunc("/blogs/{blogId}/comments", router.ProxyBlog).Methods("GET")
	protectedRouter.HandleFunc("/comments", router.ProxyBlog).Methods("POST", "GET")
	protectedRouter.HandleFunc("/comments/{id}", router.ProxyBlog).Methods("PUT")

	// User/Profile routes
	protectedRouter.HandleFunc("/getMyInfo", router.ProxyAuth).Methods("GET")
	protectedRouter.HandleFunc("/me", router.ProxyAuth).Methods("GET", "PUT")
	protectedRouter.HandleFunc("/updateMyInfo", router.ProxyAuth).Methods("PUT")

	// Tour routes
	protectedRouter.HandleFunc("/tours", router.ProxyTour).Methods("POST", "GET")
	protectedRouter.HandleFunc("/tours/{id}", router.ProxyTour).Methods("GET")
	protectedRouter.HandleFunc("/tours/my", router.ProxyTour).Methods("GET")
	protectedRouter.HandleFunc("/keypoints", router.ProxyTour).Methods("POST")
	protectedRouter.HandleFunc("/keypoints/{id}", router.ProxyTour).Methods("GET")

	// Review routes
	protectedRouter.HandleFunc("/tours/{tourId}/reviews", router.ProxyTour).Methods("POST", "GET")
	protectedRouter.HandleFunc("/tours/{tourId}/reviews/stats", router.ProxyTour).Methods("GET")
	protectedRouter.HandleFunc("/reviews/{reviewId}", router.ProxyTour).Methods("GET", "PUT", "DELETE")

	// Follower routes
	protectedRouter.HandleFunc("/followers/{followerId}/follow/{followedId}", router.ProxyFollower).Methods("POST")
	protectedRouter.HandleFunc("/followers/{followerId}/follow/{followedId}", router.ProxyFollower).Methods("DELETE")
	protectedRouter.HandleFunc("/followers/{followerId}/following", router.ProxyFollower).Methods("GET")
	protectedRouter.HandleFunc("/followers/{followerId}/followers", router.ProxyFollower).Methods("GET")
	protectedRouter.HandleFunc("/followers/{followerId}/is-following/{followedId}", router.ProxyFollower).Methods("GET")
	protectedRouter.HandleFunc("/followers/{userId}/recommendations", router.ProxyFollower).Methods("GET")
	protectedRouter.HandleFunc("/followers/{userId}/can-read/{authorId}", router.ProxyFollower).Methods("GET")
	protectedRouter.HandleFunc("/followers/{userId}/can-comment/{authorId}", router.ProxyFollower).Methods("GET")

	// Admin routes
	adminRouter := protectedRouter.PathPrefix("/admin").Subrouter()
	adminRouter.Use(middleware.AdminMiddleware)
	adminRouter.HandleFunc("/users", router.ProxyAuth).Methods("GET")
	adminRouter.HandleFunc("/users/{id}/block", router.ProxyAuth).Methods("PUT")

	port := ":8000"
	log.Println("🚀 API Gateway starting on port" + port)
	log.Println("Routes configured:")
	log.Println("  - Blog Service: /api/blogs, /api/comments")
	log.Println("  - Stakeholders Service: /api/auth, /api/users, /api/me")
	log.Println("  - Tour Service: /api/tours, /api/keypoints")
	log.Println("  - Follower Service: /api/followers/*")
	log.Println("  - Health check: /health")

	if err := http.ListenAndServe(port, muxRouter); err != nil {
		log.Fatal("Failed to start gateway: ", err)
	}
}

func healthCheck(w http.ResponseWriter, r *http.Request) {
	w.Header().Set("Content-Type", "application/json")
	w.WriteHeader(http.StatusOK)
	w.Write([]byte(`{"status":"healthy","gateway":"api-gateway","version":"1.0"}`))
}
