package service

import (
	"blog-service/models"
	"blog-service/repo"
	"errors"

	"github.com/google/uuid"
	"gorm.io/gorm"
)

var ErrBlogNotFound = errors.New("blog not found")
var ErrLikeAlreadyExists = errors.New("user already liked this blog")
var ErrLikeNotFound = errors.New("like not found")

type LikeService struct {
	LikeRepository *repo.LikeRepository
	BlogRepository *repo.BlogRepository
}

func (s *LikeService) LikeBlog(blogID string, userID string) (int64, error) {
	blogUUID, err := uuid.Parse(blogID)
	if err != nil {
		return 0, err
	}
	_, err = s.BlogRepository.GetByID(blogUUID)
	if err != nil {
		if errors.Is(err, gorm.ErrRecordNotFound) {
			return 0, ErrBlogNotFound
		}
		return 0, err
	}

	_, err = s.LikeRepository.GetByBlogAndUser(blogUUID, userID)
	if err == nil {
		return 0, ErrLikeAlreadyExists
	}
	if !errors.Is(err, gorm.ErrRecordNotFound) {
		return 0, err
	}

	like := &models.Like{
		BlogID: blogUUID,
		UserID: userID,
	}

	err = s.LikeRepository.CreateLike(like)
	if err != nil {
		return 0, err
	}

	return s.LikeRepository.CountByBlogID(blogUUID)
}

func (s *LikeService) UnlikeBlog(blogID string, userID string) (int64, error) {
	blogUUID, err := uuid.Parse(blogID)
	if err != nil {
		return 0, err
	}

	like, err := s.LikeRepository.GetByBlogAndUser(blogUUID, userID)
	if err != nil {
		if errors.Is(err, gorm.ErrRecordNotFound) {
			return 0, ErrLikeNotFound
		}
		return 0, err
	}

	err = s.LikeRepository.DeleteLikeByID(like.ID)
	if err != nil {
		return 0, err
	}

	return s.LikeRepository.CountByBlogID(blogUUID)
}
