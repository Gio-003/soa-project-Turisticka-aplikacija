package dto

type CommentDTO struct {
	Content string `json:"content" binding:"required"`
}
