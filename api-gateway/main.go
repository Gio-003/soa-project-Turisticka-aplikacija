package main

import (
	"api-gateway/middleware"
	"api-gateway/router"
	"log"
	"net/http"

	"github.com/gorilla/mux"
)

func main() {
	muxRouter := mux.NewRouter()

	// CORS middleware
	muxRouter.Use(middleware.CORSMiddleware)

	// Health check (no auth needed)
	muxRouter.HandleFunc("/health", healthCheck).Methods("GET")

	// Auth routes (no JWT required)
	muxRouter.HandleFunc("/api/auth/login", router.ProxyAuth).Methods("POST")
	muxRouter.HandleFunc("/api/auth/signup", router.ProxyAuth).Methods("POST")

	// Protected routes (JWT required)
	protectedRouter := muxRouter.PathPrefix("/api").Subrouter()
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
