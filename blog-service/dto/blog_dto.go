package dto

type CreateBlogDTO struct {
	Title       string   `json:"title" binding:"required"`
	Description string   `json:"description" binding:"required"`
	ImageURL    []string `json:"image_url"`
}
