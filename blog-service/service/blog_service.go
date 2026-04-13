package service

import (
	"blog-service/dto"
	"blog-service/models"
	"blog-service/repo"

	"github.com/google/uuid"
)

type BlogService struct {
	Repository     *repo.BlogRepository
	LikeRepository *repo.LikeRepository
}

func (s *BlogService) CreateBlog(createBlogDto *dto.CreateBlogDTO, authorId string) (*models.Blog, error) {
	blog := &models.Blog{
		Title:       createBlogDto.Title,
		Description: createBlogDto.Description,
		ImageURL:    createBlogDto.ImageURL,
		AuthorID:    authorId,
	}
	err := s.Repository.CreateBlog(blog)
	if err != nil {
		return nil, err
	}
	blog.LikesCount = 0
	return blog, nil
}

func (s *BlogService) GetBlogByID(blogID string) (*models.Blog, error) {
	blogUUID, err := uuid.Parse(blogID)
	if err != nil {
		return nil, err
	}
	blog, err := s.Repository.GetByID(blogUUID)
	if err != nil {
		return nil, err
	}
	if s.LikeRepository != nil {
		likesCount, err := s.LikeRepository.CountByBlogID(blog.ID)
		if err != nil {
			return nil, err
		}
		blog.LikesCount = likesCount
	}
	return blog, nil
}

func (s *BlogService) GetAllBlogs() ([]models.Blog, error) {
	blogs, err := s.Repository.GetAll()
	if err != nil {
		return nil, err
	}
	if s.LikeRepository != nil {
		for i := range blogs {
			likesCount, countErr := s.LikeRepository.CountByBlogID(blogs[i].ID)
			if countErr != nil {
				return nil, countErr
			}
			blogs[i].LikesCount = likesCount
		}
	}
	return blogs, nil
}
