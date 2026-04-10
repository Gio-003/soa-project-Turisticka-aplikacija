package dto

type UpdateCommentDTO struct {
	Content string `json:"content" binding:"required"`
}
