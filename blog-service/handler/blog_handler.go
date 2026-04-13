package handler

import (
	"blog-service/dto"
	"blog-service/service"
	"encoding/json"
	"net/http"

	"github.com/gorilla/mux"
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
	autorId := GetUserIDFromRequest(r) // This should be replaced with actual logic to get the author ID, e.g., from authentication context
	blog, err := h.Service.CreateBlog(&createBlogDto, autorId)
	if err != nil {
		http.Error(w, "Failed to create blog", http.StatusInternalServerError)
		return
	}
	w.Header().Set("Content-Type", "application/json")
	w.WriteHeader(http.StatusCreated)
	json.NewEncoder(w).Encode(blog)
}

func (h *BlogHandler) GetBlogByID(w http.ResponseWriter, r *http.Request) {
	if r.Method != http.MethodGet {
		http.Error(w, "Method not allowed", http.StatusMethodNotAllowed)
		return
	}
	vars := mux.Vars(r)
	blogID := vars["id"]

	blog, err := h.Service.GetBlogByID(blogID)
	if err != nil {
		http.Error(w, "Failed to retrieve blog", http.StatusInternalServerError)
		return
	}

	w.Header().Set("Content-Type", "application/json")
	json.NewEncoder(w).Encode(blog)
}

func (h *BlogHandler) GetAllBlogs(w http.ResponseWriter, r *http.Request) {
	if r.Method != http.MethodGet {
		http.Error(w, "Method not allowed", http.StatusMethodNotAllowed)
		return
	}

	blogs, err := h.Service.GetAllBlogs()
	if err != nil {
		http.Error(w, "Failed to retrieve blogs", http.StatusInternalServerError)
		return
	}

	w.Header().Set("Content-Type", "application/json")
	json.NewEncoder(w).Encode(blogs)
}
