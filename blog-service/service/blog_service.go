package service

import (
	"blog-service/dto"
	"blog-service/models"
	"blog-service/repo"
)

type BlogService struct {
	Repository *repo.BlogRepository
}

func (s *BlogService) CreateBlog(createBlogDto *dto.CreateBlogDTO, authorId string) (*models.Blog, error) {
	blog := &models.Blog{
		Title:       createBlogDto.Title,
		Description: createBlogDto.Description,
		ImageURL:    createBlogDto.ImageURL,
		AuthorID:    authorId,
	}
	err := s.Repository.Create(blog)
	if err != nil {
		return nil, err
	}
	return blog, nil
}

func (s *BlogService) GetBlogByID(blogID string) (*models.Blog, error) {
	// Repozitorijum sada interno konvertuje string u ObjectID
	return s.Repository.GetByID(blogID)
}

// GetAllBlogs vraća sve blogove.
func (s *BlogService) GetAllBlogs() ([]models.Blog, error) {
	return s.Repository.GetAll()
}

// LikeBlog poziva metodu za lajkovanje iz repozitorijuma.
func (s *BlogService) LikeBlog(blogID, userID string) error {
	return s.Repository.Like(blogID, userID)
}
func (s *BlogService) UnlikeBlog(blogID, userID string) error {
	return s.Repository.Unlike(blogID, userID)
}

// AddComment poziva metodu za dodavanje komentara iz repozitorijuma.
func (s *BlogService) AddComment(blogID string, createCommentDTO *dto.CommentDTO, authorId string) error {
	comment := &models.Comment{
		Content:  createCommentDTO.Content,
		AuthorID: authorId,
	}
	return s.Repository.AddComment(blogID, comment)
}
