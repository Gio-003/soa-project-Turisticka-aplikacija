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

        // CREATE
        public Tour Create(Tour tour)
        {
            _context.Tours.Add(tour);
            _context.SaveChanges();
            return tour;
        }

        // GET BY ID
        public Tour GetById(Guid id)
        {
            return _context.Tours
                .Include(t => t.Tags)
                .Include(t => t.KeyPoints)
                .FirstOrDefault(t => t.Id == id);
        }

        // UPDATE
        public Tour Update(Tour tour)
        {
            _context.Tours.Update(tour);
            _context.SaveChanges();
            return tour;
        }

        // DELETE
        public Tour Delete(Guid id)
        {
            var tour = _context.Tours
                .Include(t => t.Tags)
                .Include(t => t.KeyPoints)
                .FirstOrDefault(t => t.Id == id);

            if (tour != null)
            {
                _context.Tours.Remove(tour);
                _context.SaveChanges();
            }

            return tour;
        }

        // GET ALL
        public List<Tour> GetAll()
        {
            return _context.Tours
                .Include(t => t.Tags)
                .Include(t => t.KeyPoints)
                .ToList();
        }

        // GET BY AUTHOR
        public List<Tour> GetByAuthorId(Guid authorId)
        {
            return _context.Tours
                .Include(t => t.Tags)
                .Include(t => t.KeyPoints)
                .Where(t => t.AuthorId == authorId)
                .ToList();
        }

        // OPTIONAL: GET ONLY PUBLISHED
        public List<Tour> GetPublished()
        {
            return _context.Tours
                .Include(t => t.Tags)
                .Include(t => t.KeyPoints)
                .Where(t => t.Status == Enum.TourStatus.Published)
                .ToList();
        }
    }
}