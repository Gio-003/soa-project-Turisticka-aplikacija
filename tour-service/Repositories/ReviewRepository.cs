using tour_service.Models;
using tour_service.Data;

namespace tour_service.Repositories
{
    public class ReviewRepository
    {
        private readonly AppDbContext _context;

        public ReviewRepository(AppDbContext context)
        {
            _context = context;
        }

        public Review Create(Review review)
        {
            _context.Reviews.Add(review);
            _context.SaveChanges();
            return review;
        }

        public Review GetById(Guid id)
        {
            return _context.Reviews.Find(id);
        }

        public List<Review> GetByTourId(Guid tourId)
        {
            return _context.Reviews
                .Where(r => r.TourId == tourId)
                .OrderByDescending(r => r.CreatedAt)
                .ToList();
        }

        public List<Review> GetAll()
        {
            return _context.Reviews.ToList();
        }
    }
}
