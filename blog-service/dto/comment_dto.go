package dto

type CommentDTO struct {
	BlogID  string `json:"blogId" binding:"required"`
	Content string `json:"content" binding:"required"`
}
