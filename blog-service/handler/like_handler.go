package handler

import (
	"blog-service/service"
	"encoding/json"
	"fmt"
	"net/http"
	"os"
	"strings"

	"github.com/golang-jwt/jwt/v5"
	"github.com/gorilla/mux"
)

type LikeHandler struct {
	Service *service.LikeService
}

func (h *LikeHandler) LikeBlog(w http.ResponseWriter, r *http.Request) {
	if r.Method != http.MethodPost {
		http.Error(w, "Method not allowed", http.StatusMethodNotAllowed)
		return
	}

	vars := mux.Vars(r)
	blogID := vars["blogId"]
	userID := getUserIDFromRequest(r)

	likesCount, err := h.Service.LikeBlog(blogID, userID)
	if err != nil {
		switch err {
		case service.ErrLikeAlreadyExists:
			http.Error(w, err.Error(), http.StatusConflict)
			return
		case service.ErrBlogNotFound:
			http.Error(w, err.Error(), http.StatusNotFound)
			return
		default:
			http.Error(w, "Failed to like blog", http.StatusInternalServerError)
			return
		}
	}

	w.Header().Set("Content-Type", "application/json")
	w.WriteHeader(http.StatusCreated)
	json.NewEncoder(w).Encode(map[string]int64{"likesCount": likesCount})
}

func (h *LikeHandler) UnlikeBlog(w http.ResponseWriter, r *http.Request) {
	if r.Method != http.MethodDelete {
		http.Error(w, "Method not allowed", http.StatusMethodNotAllowed)
		return
	}

	vars := mux.Vars(r)
	blogID := vars["blogId"]
	userID := getUserIDFromRequest(r)

	likesCount, err := h.Service.UnlikeBlog(blogID, userID)
	if err != nil {
		switch err {
		case service.ErrLikeNotFound:
			http.Error(w, err.Error(), http.StatusNotFound)
			return
		default:
			http.Error(w, "Failed to unlike blog", http.StatusInternalServerError)
			return
		}
	}

	w.Header().Set("Content-Type", "application/json")
	json.NewEncoder(w).Encode(map[string]int64{"likesCount": likesCount})
}

func getUserIDFromRequest(r *http.Request) string {
	authHeader := r.Header.Get("Authorization")
	if strings.HasPrefix(authHeader, "Bearer ") {
		tokenString := strings.TrimPrefix(authHeader, "Bearer ")
		secret := os.Getenv("JWT_SECRET")
		if secret == "" {
			secret = "somesecret-key-for-jwt-token-has-to-be-512-bits-long-1234567890123456789"
		}

		token, err := jwt.Parse(tokenString, func(token *jwt.Token) (interface{}, error) {
			if _, ok := token.Method.(*jwt.SigningMethodHMAC); !ok {
				return nil, fmt.Errorf("unexpected signing method")
			}
			return []byte(secret), nil
		})

		if err == nil && token != nil && token.Valid {
			if claims, ok := token.Claims.(jwt.MapClaims); ok {
				if userIDValue, exists := claims["userId"]; exists {
					switch value := userIDValue.(type) {
					case float64:
						return fmt.Sprintf("%.0f", value)
					case string:
						if value != "" {
							return value
						}
					}
				}
			}
		}
	}

	userID := r.Header.Get("X-User-Id")
	if userID == "" {
		return "123"
	}
	return userID
}
