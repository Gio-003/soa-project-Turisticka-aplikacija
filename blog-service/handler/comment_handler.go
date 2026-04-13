package handler

import (
	"blog-service/dto"
	"blog-service/service"
	"encoding/json"
	"net/http"

	"github.com/gorilla/mux"
)

type CommentHandler struct {
	Service *service.CommentService
}

func (h *CommentHandler) CreateComment(w http.ResponseWriter, r *http.Request) {
	if r.Method != http.MethodPost {
		http.Error(w, "Method not allowed", http.StatusMethodNotAllowed)
		return
	}
	var createCommentDto dto.CommentDTO
	err := json.NewDecoder(r.Body).Decode(&createCommentDto)
	if err != nil {
		http.Error(w, "Invalid request body", http.StatusBadRequest)
		return
	}
	autorId := GetUserIDFromRequest(r) // This should be replaced with actual logic to get the author ID, e.g., from authentication context
	comment, err := h.Service.CreateComment(&createCommentDto, autorId)
	if err != nil {
		http.Error(w, "Failed to create comment", http.StatusInternalServerError)
		return
	}
	w.Header().Set("Content-Type", "application/json")
	w.WriteHeader(http.StatusCreated)
	json.NewEncoder(w).Encode(comment)
}

func (h *CommentHandler) GetCommentsByBlogID(w http.ResponseWriter, r *http.Request) {
	if r.Method != http.MethodGet {
		http.Error(w, "Method not allowed", http.StatusMethodNotAllowed)
		return
	}
	vars := mux.Vars(r)
	blogID := vars["blogId"]
	comments, err := h.Service.GetCommentsByBlogID(blogID)
	if err != nil {
		http.Error(w, "Failed to retrieve comments", http.StatusInternalServerError)
		return
	}
	w.Header().Set("Content-Type", "application/json")
	json.NewEncoder(w).Encode(comments)
}

func (h *CommentHandler) GetAllComments(w http.ResponseWriter, r *http.Request) {
	if r.Method != http.MethodGet {
		http.Error(w, "Method not allowed", http.StatusMethodNotAllowed)
		return
	}
	comments, err := h.Service.GetAllComments()
	if err != nil {
		http.Error(w, "Failed to retrieve comments", http.StatusInternalServerError)
		return
	}
	w.Header().Set("Content-Type", "application/json")
	json.NewEncoder(w).Encode(comments)
}

func (h *CommentHandler) UpdateComment(w http.ResponseWriter, r *http.Request) {
	if r.Method != http.MethodPut {
		http.Error(w, "Method not allowed", http.StatusMethodNotAllowed)
		return
	}
	vars := mux.Vars(r)
	commentId := vars["id"]
	var updateCommentDto dto.UpdateCommentDTO
	err := json.NewDecoder(r.Body).Decode(&updateCommentDto)
	if err != nil {
		http.Error(w, "Invalid request body", http.StatusBadRequest)
		return
	}
	updatedComment, err := h.Service.UpdateComment(commentId, &updateCommentDto)
	if err != nil {
		http.Error(w, "Failed to update comment", http.StatusInternalServerError)
		return
	}
	w.Header().Set("Content-Type", "application/json")
	json.NewEncoder(w).Encode(updatedComment)
}
