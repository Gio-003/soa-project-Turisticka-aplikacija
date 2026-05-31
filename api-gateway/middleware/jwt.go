package middleware

import (
	"api-gateway/config"
	"fmt"
	"log"
	"net/http"
	"strings"

	"github.com/golang-jwt/jwt/v5"
)

type CustomClaims struct {
	UserID string  `json:"sub"`
	UserId float64 `json:"userId"`
	Role   string  `json:"role"`
	jwt.RegisteredClaims
}

func JWTMiddleware(next http.Handler) http.Handler {
	return http.HandlerFunc(func(w http.ResponseWriter, r *http.Request) {
		authHeader := r.Header.Get("Authorization")
		if authHeader == "" {
			log.Printf("JWT missing Authorization header for %s %s", r.Method, r.URL.Path)
			http.Error(w, `{"error":"missing authorization header"}`, http.StatusUnauthorized)
			return
		}

		parts := strings.SplitN(authHeader, " ", 2)
		if len(parts) != 2 || parts[0] != "Bearer" {
			log.Printf("JWT invalid Authorization format for %s %s", r.Method, r.URL.Path)
			http.Error(w, `{"error":"invalid authorization format"}`, http.StatusUnauthorized)
			return
		}

		tokenString := parts[1]
		token, err := jwt.ParseWithClaims(tokenString, &CustomClaims{}, func(token *jwt.Token) (interface{}, error) {
			return config.JWTSecret, nil
		})

		if err != nil || !token.Valid {
			log.Printf("JWT invalid token for %s %s: %v", r.Method, r.URL.Path, err)
			http.Error(w, `{"error":"invalid token"}`, http.StatusUnauthorized)
			return
		}

		claims, ok := token.Claims.(*CustomClaims)
		if !ok {
			log.Printf("JWT invalid claims for %s %s", r.Method, r.URL.Path)
			http.Error(w, `{"error":"invalid token claims"}`, http.StatusUnauthorized)
			return
		}

		r.Header.Set("X-User-ID", fmt.Sprintf("%d", int(claims.UserId)))
		r.Header.Set("X-User-Role", claims.Role)
		r.Header.Set("X-Username", claims.UserID)

		next.ServeHTTP(w, r)
	})
}

func JWTMiddlewareFunc(next http.HandlerFunc) http.Handler {
	return http.HandlerFunc(func(w http.ResponseWriter, r *http.Request) {
		if r.Method == "OPTIONS" {
			w.WriteHeader(http.StatusOK)
			return
		}
		JWTMiddleware(http.HandlerFunc(next)).ServeHTTP(w, r)
	})
}

func AdminMiddlewareFunc(next http.HandlerFunc) http.HandlerFunc {
	return func(w http.ResponseWriter, r *http.Request) {
		AdminMiddleware(http.HandlerFunc(next)).ServeHTTP(w, r)
	}
}
