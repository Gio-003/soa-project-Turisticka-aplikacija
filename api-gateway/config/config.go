package config

import (
	"os"
)

// ServiceURLs holds the backend service URLs
var ServiceURLs = map[string]string{
	"blog":        getEnv("BLOG_SERVICE_URL", "http://blog:8080"),
	"stakeholders": getEnv("STAKEHOLDERS_SERVICE_URL", "http://stakeholders:8080"),
}

// JWT Configuration
var JWTSecret = []byte(getEnv("JWT_SECRET", "your-secret-key-change-in-production"))

func getEnv(key, defaultValue string) string {
	if value, exists := os.LookupEnv(key); exists {
		return value
	}
	return defaultValue
}
