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
		//ImageURL:    createBlogDto.ImageURL,
		AuthorID: authorId,
	}
	err := s.Repository.CreateBlog(blog)
	if err != nil {
		return nil, err
	}
	return blog, nil
}
