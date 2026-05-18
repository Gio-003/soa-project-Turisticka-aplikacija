using tour_service.Repositories;
using tour_service.Models;
using tour_service.DTO;

namespace tour_service.Services
{
    public class ReviewService
    {
        private readonly ReviewRepository _repository;

        public ReviewService(ReviewRepository repository)
        {
            _repository = repository;
        }

        public ReviewResponse CreateReview(Guid tourId, int touristId, string touristUsername, CreateReviewRequest request)
        {
            if (tourId == Guid.Empty)
                throw new ArgumentException("Invalid tour id");

            if (request.Rating < 1 || request.Rating > 5)
                throw new ArgumentException("Rating must be between 1 and 5");

            var review = new Review
            {
                Id = Guid.NewGuid(),
                TourId = tourId,
                Rating = request.Rating,
                Comment = request.Comment,
                TouristId = touristId,
                TouristUsername = touristUsername,
                VisitDate = DateTime.SpecifyKind(request.VisitDate, DateTimeKind.Utc),                
                CreatedAt = DateTime.UtcNow,
                Images = request.Images ?? new List<string>()
            };

            var created = _repository.Create(review);
            return MapToResponse(created);
        }

        public List<ReviewResponse> GetReviewsByTourId(Guid tourId)
        {
            var reviews = _repository.GetByTourId(tourId);
            return reviews.Select(MapToResponse).ToList();
        }

        public ReviewResponse GetReviewById(Guid id)
        {
            var review = _repository.GetById(id);
            if (review == null)
                return null;
            return MapToResponse(review);
        }

        private ReviewResponse MapToResponse(Review review)
        {
            return new ReviewResponse
            {
                Id = review.Id,
                TourId = review.TourId,
                Rating = review.Rating,
                Comment = review.Comment,
                TouristId = review.TouristId,
                TouristUsername = review.TouristUsername,
                VisitDate = review.VisitDate,
                CreatedAt = review.CreatedAt,
                Images = review.Images
            };
        }
    }
}
