using tour_service.Data;
using tour_service.Models;
using Microsoft.EntityFrameworkCore;

namespace tour_service.Repositories
{
    public class ReviewRepository
    {
        private readonly AppDbContext _context;

        public ReviewRepository(AppDbContext context)
        {
            _context = context;
        }

        public Review Add(Review review)
        {
            _context.Reviews.Add(review);
            _context.SaveChanges();
            return review;
        }

        public Review GetById(Guid id)
        {
            return _context.Reviews.FirstOrDefault(r => r.Id == id);
        }

        public List<Review> GetReviewsByTourId(Guid tourId)
        {
            return _context.Reviews
                .Where(r => r.TourId == tourId)
                .OrderByDescending(r => r.ReviewDate)
                .ToList();
        }

        public List<Review> GetReviewsByUserId(Guid userId)
        {
            return _context.Reviews
                .Where(r => r.ReviewerUserId == userId)
                .OrderByDescending(r => r.ReviewDate)
                .ToList();
        }

        public Review Update(Review review)
        {
            _context.Reviews.Update(review);
            _context.SaveChanges();
            return review;
        }

        public bool Delete(Guid id)
        {
            var review = GetById(id);
            if (review == null) return false;

            _context.Reviews.Remove(review);
            _context.SaveChanges();
            return true;
        }

        public double GetAverageRating(Guid tourId)
        {
            var reviews = GetReviewsByTourId(tourId);
            if (reviews.Count == 0) return 0;

            return reviews.Average(r => (int)r.Rating);
        }

        public int GetReviewCount(Guid tourId)
        {
            return _context.Reviews.Count(r => r.TourId == tourId);
        }
    }
}
