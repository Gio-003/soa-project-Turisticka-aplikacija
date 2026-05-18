using tour_service.Data;
using tour_service.Models;
using tour_service.DTO;
using tour_service.Repositories;
using System.Text.Json;

namespace tour_service.Services
{
    public class ReviewService
    {
        private readonly ReviewRepository _repository;
        private readonly AppDbContext _context;

        public ReviewService(ReviewRepository repository, AppDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public ReviewResponseDTO CreateReview(Guid tourId, Guid userId, CreateReviewDTO request, string reviewerName, string reviewerProfilePicture)
        {
            // Validacija
            if (string.IsNullOrWhiteSpace(request.Comment))
                throw new ArgumentException("Comment is required");

            if (request.Rating < 1 || request.Rating > 5)
                throw new ArgumentException("Rating must be between 1 and 5");

            var review = new Review
            {
                Id = Guid.NewGuid(),
                TourId = tourId,
                ReviewerUserId = userId,
                Rating = (Enum.ReviewRating)request.Rating,
                Comment = request.Comment,
                VisitDate = request.VisitDate,
                ReviewDate = DateTime.UtcNow,
                Images = JsonSerializer.Serialize(request.Images),
                ReviewerName = reviewerName,
                ReviewerProfilePicture = reviewerProfilePicture
            };

            _repository.Add(review);
            return ToDTO(review);
        }

        public ReviewResponseDTO GetReviewById(Guid reviewId)
        {
            var review = _repository.GetById(reviewId);
            if (review == null)
                throw new KeyNotFoundException("Review not found");

            return ToDTO(review);
        }

        public List<ReviewResponseDTO> GetReviewsByTourId(Guid tourId)
        {
            var reviews = _repository.GetReviewsByTourId(tourId);
            return reviews.Select(ToDTO).ToList();
        }

        public List<ReviewResponseDTO> GetReviewsByUserId(Guid userId)
        {
            var reviews = _repository.GetReviewsByUserId(userId);
            return reviews.Select(ToDTO).ToList();
        }

        public ReviewResponseDTO UpdateReview(Guid reviewId, CreateReviewDTO request, Guid userId)
        {
            var review = _repository.GetById(reviewId);
            if (review == null)
                throw new KeyNotFoundException("Review not found");

            if (review.ReviewerUserId != userId)
                throw new UnauthorizedAccessException("You can only edit your own reviews");

            review.Rating = (Enum.ReviewRating)request.Rating;
            review.Comment = request.Comment;
            review.VisitDate = request.VisitDate;
            review.Images = JsonSerializer.Serialize(request.Images);

            _repository.Update(review);
            return ToDTO(review);
        }

        public bool DeleteReview(Guid reviewId, Guid userId)
        {
            var review = _repository.GetById(reviewId);
            if (review == null)
                throw new KeyNotFoundException("Review not found");

            if (review.ReviewerUserId != userId)
                throw new UnauthorizedAccessException("You can only delete your own reviews");

            return _repository.Delete(reviewId);
        }

        public double GetAverageRating(Guid tourId)
        {
            return _repository.GetAverageRating(tourId);
        }

        public int GetReviewCount(Guid tourId)
        {
            return _repository.GetReviewCount(tourId);
        }

        // Helper methods
        private ReviewResponseDTO ToDTO(Review review)
        {
            var images = new List<string>();
            try
            {
                images = JsonSerializer.Deserialize<List<string>>(review.Images) ?? new List<string>();
            }
            catch { }

            return new ReviewResponseDTO
            {
                Id = review.Id,
                TourId = review.TourId,
                ReviewerUserId = review.ReviewerUserId,
                Rating = (int)review.Rating,
                Comment = review.Comment,
                VisitDate = review.VisitDate,
                ReviewDate = review.ReviewDate,
                Images = images,
                ReviewerName = review.ReviewerName,
                ReviewerProfilePicture = review.ReviewerProfilePicture
            };
        }
    }
}
