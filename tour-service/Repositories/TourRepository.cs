using Microsoft.EntityFrameworkCore;
using tour_service.Data;
using tour_service.Models;

namespace tour_service.Repositories
{
    public class TourRepository
    {
        private readonly AppDbContext _context;

        public TourRepository(AppDbContext context)
        {
            _context = context;
        }

        public Tour Create(Tour tour)
        {
            _context.Tours.Add(tour);
            _context.SaveChanges();
            return tour;
        }

        public Tour GetById(Guid id)
        {
            return _context.Tours
                .Include(t => t.Tags)
                .Include(t => t.KeyPoints)
                .Include(t => t.Durations)
                .FirstOrDefault(t => t.Id == id);
        }

        public Tour Update(Tour tour)
        {
            _context.Tours.Update(tour);
            _context.SaveChanges();
            return tour;
        }

        public Tour Delete(Guid id)
        {
            var tour = _context.Tours
                .Include(t => t.Tags)
                .Include(t => t.KeyPoints)
                .Include(t => t.Durations)
                .FirstOrDefault(t => t.Id == id);

            if (tour != null)
            {
                _context.Tours.Remove(tour);
                _context.SaveChanges();
            }

            return tour;
        }

        public List<Tour> GetAll()
        {
            return _context.Tours
                .Include(t => t.Tags)
                .Include(t => t.KeyPoints)
                .Include(t => t.Durations)
                .ToList();
        }

        public List<Tour> GetByAuthorId(int authorId)
        {
            return _context.Tours
                .Include(t => t.Tags)
                .Include(t => t.KeyPoints)
                .Include(t => t.Durations)
                .Where(t => t.AuthorId == authorId)
                .ToList();
        }

        public List<Tour> GetPublished()
        {
            return _context.Tours
                .Include(t => t.Tags)
                .Include(t => t.KeyPoints)
                .Include(t => t.Durations)
                .Where(t => t.Status == Enum.TourStatus.Published)
                .ToList();
        }
    }
}