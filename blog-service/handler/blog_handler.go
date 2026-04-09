package handler

import (
	"blog-service/dto"
	"blog-service/service"
	"encoding/json"
	"net/http"
)

type BlogHandler struct {
	Service *service.BlogService
}

func (h *BlogHandler) CreateBlog(w http.ResponseWriter, r *http.Request) {
	if r.Method != http.MethodPost {
		http.Error(w, "Method not allowed", http.StatusMethodNotAllowed)
		return
	}
	var createBlogDto dto.CreateBlogDTO
	err := json.NewDecoder(r.Body).Decode(&createBlogDto)
	if err != nil {
		http.Error(w, "Invalid request body", http.StatusBadRequest)
		return
	}
	autorId := "123" // This should be replaced with actual logic to get the author ID, e.g., from authentication context
	blog, err := h.Service.CreateBlog(&createBlogDto, autorId)
	if err != nil {
		http.Error(w, "Failed to create blog", http.StatusInternalServerError)
		return
	}
	w.Header().Set("Content-Type", "application/json")
	w.WriteHeader(http.StatusCreated)
	json.NewEncoder(w).Encode(blog)
}
