package handler

import (
	"blog-service/dto"
	"blog-service/service"
	"blog-service/models" 
	"encoding/json"
	"net/http"

	"github.com/gorilla/mux"
)

type BlogHandler struct {
	Service *service.BlogService
}

type BlogResponse struct {
    BlogId string `json:"blogId"`
}
// --- Blog Metode ---

func (h *BlogHandler) CreateBlog(w http.ResponseWriter, r *http.Request) {
	var createBlogDto dto.CreateBlogDTO
	if err := json.NewDecoder(r.Body).Decode(&createBlogDto); err != nil {
		http.Error(w, "Invalid request body", http.StatusBadRequest)
		return
	}

	authorId := GetUserIDFromRequest(r)
	blog, err := h.Service.CreateBlog(&createBlogDto, authorId)
	if err != nil {
		http.Error(w, "Failed to create blog", http.StatusInternalServerError)
		return
	}

	w.Header().Set("Content-Type", "application/json")
	w.WriteHeader(http.StatusCreated)
	json.NewEncoder(w).Encode(blog)
}

func (h *BlogHandler) CreateWelcomeBlog(w http.ResponseWriter, r *http.Request) {
    userId := r.URL.Query().Get("userId")

    blog := &models.Blog{
        Title:       "Hello everyone!",
        Description: "I am a new tour guide. Soon, you can expect tours guided by me.",
        AuthorID:    userId,
    }

    err := h.Service.CreateWelcomeBlog(blog)
    if err != nil {
        http.Error(w, err.Error(), http.StatusInternalServerError)
        return
    }

    w.Header().Set("Content-Type", "text/plain")
	w.WriteHeader(http.StatusCreated)
	w.Write([]byte(blog.ID.Hex()))
}


func (h *BlogHandler) DeleteBlog(w http.ResponseWriter, r *http.Request) {
    vars := mux.Vars(r)
    id := vars["id"]

    err := h.Service.DeleteBlog(id)
    if err != nil {
        http.Error(w, err.Error(), http.StatusInternalServerError)
        return
    }

    w.WriteHeader(http.StatusNoContent)
}

func (h *BlogHandler) GetBlogByID(w http.ResponseWriter, r *http.Request) {
	vars := mux.Vars(r)
	blogID := vars["id"]

	blog, err := h.Service.GetBlogByID(blogID)
	if err != nil {
		http.Error(w, "Blog not found", http.StatusNotFound)
		return
	}

	w.Header().Set("Content-Type", "application/json")
	json.NewEncoder(w).Encode(blog)
}

func (h *BlogHandler) GetAllBlogs(w http.ResponseWriter, r *http.Request) {
	blogs, err := h.Service.GetAllBlogs()
	if err != nil {
		http.Error(w, "Failed to retrieve blogs", http.StatusInternalServerError)
		return
	}

	w.Header().Set("Content-Type", "application/json")
	json.NewEncoder(w).Encode(blogs)
}

// --- Like Metode (premeštene ovde) ---

func (h *BlogHandler) LikeBlog(w http.ResponseWriter, r *http.Request) {
	vars := mux.Vars(r)
	blogID := vars["blogId"]
	userID := GetUserIDFromRequest(r)

	if err := h.Service.LikeBlog(blogID, userID); err != nil {
		http.Error(w, "Failed to like blog", http.StatusInternalServerError)
		return
	}

	w.WriteHeader(http.StatusOK)
	json.NewEncoder(w).Encode(map[string]string{"status": "blog liked"})
}

func (h *BlogHandler) UnlikeBlog(w http.ResponseWriter, r *http.Request) {
	vars := mux.Vars(r)
	blogID := vars["blogId"]
	userID := GetUserIDFromRequest(r)

	if err := h.Service.UnlikeBlog(blogID, userID); err != nil {
		http.Error(w, "Failed to unlike blog", http.StatusInternalServerError)
		return
	}

	w.WriteHeader(http.StatusOK)
	json.NewEncoder(w).Encode(map[string]string{"status": "blog unliked"})
}

// --- Comment Metode (premeštene ovde) ---

func (h *BlogHandler) AddComment(w http.ResponseWriter, r *http.Request) {
	vars := mux.Vars(r)
	blogID := vars["blogId"]

	var createCommentDto dto.CommentDTO
	if err := json.NewDecoder(r.Body).Decode(&createCommentDto); err != nil {
		http.Error(w, "Invalid request body", http.StatusBadRequest)
		return
	}
	authorId := GetUserIDFromRequest(r)
	if err := h.Service.AddComment(blogID, &createCommentDto, authorId); err != nil {
		http.Error(w, "Failed to add comment", http.StatusInternalServerError)
		return
	}

	w.WriteHeader(http.StatusCreated)
	json.NewEncoder(w).Encode(createCommentDto)
}
