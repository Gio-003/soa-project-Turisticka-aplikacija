package config

import (
	"os"
)

// ServiceURLs holds the backend service URLs
var ServiceURLs = map[string]string{
	"blog":          getEnv("BLOG_SERVICE_URL", "http://blog-service:8080"),
	"stakeholders":  getEnv("STAKEHOLDERS_SERVICE_URL", "http://stakeholders-service:8080"),
	"tour":          getEnv("TOUR_SERVICE_URL", "http://tour-service:8080"),
	"follower":      getEnv("FOLLOWER_SERVICE_URL", "http://follower-service:8083"),

	"stakeholders":  getEnv("STAKEHOLDERS_SERVICE_URL", "http://localhost:8080"),
	"tour":          getEnv("TOUR_SERVICE_URL", "http://localhost:55814"),
}

// JWT Configuration
//var JWTSecret = []byte(getEnv("JWT_SECRET", "your-secret-key-change-in-production"))
var JWTSecret = []byte(getEnv("JWT_SECRET", "somesecret-key-for-jwt-token-has-to-be-512-bits-long-1234567890123456789"))

func getEnv(key, defaultValue string) string {
	if value, exists := os.LookupEnv(key); exists {
		return value
	}
	return defaultValue
}
