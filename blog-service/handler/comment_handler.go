package handler

import (
	"blog-service/dto"
	"blog-service/service"
	"encoding/json"
	"net/http"
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
	autorId := "123" // This should be replaced with actual logic to get the author ID, e.g., from authentication context
	comment, err := h.Service.CreateComment(&createCommentDto, autorId)
	if err != nil {
		http.Error(w, "Failed to create comment", http.StatusInternalServerError)
		return
	}
	w.Header().Set("Content-Type", "application/json")
	w.WriteHeader(http.StatusCreated)
	json.NewEncoder(w).Encode(comment)
}
