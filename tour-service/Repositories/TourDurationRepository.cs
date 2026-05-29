using Microsoft.EntityFrameworkCore;
using tour_service.Data;
using tour_service.Models;

namespace tour_service.Repositories
{
    public class TourDurationRepository
    {
        private readonly AppDbContext _context;

        public TourDurationRepository(AppDbContext context)
        {
            _context = context;
        }

        // CREATE (single)
        public TourDuration Create(TourDuration duration)
        {
            _context.TourDurations.Add(duration);
            _context.SaveChanges();
            return duration;
        }

        // CREATE (bulk)
        public List<TourDuration> CreateRange(List<TourDuration> durations)
        {
            _context.TourDurations.AddRange(durations);
            _context.SaveChanges();
            return durations;
        }

        // GET BY ID
        public TourDuration GetById(Guid id)
        {
            return _context.TourDurations
                .Include(td => td.Tour)
                .FirstOrDefault(td => td.Id == id);
        }

        // GET BY TOUR ID
        public List<TourDuration> GetByTourId(Guid tourId)
        {
            return _context.TourDurations
                .Where(td => td.TourId == tourId)
                .ToList();
        }

        // UPDATE
        public TourDuration Update(TourDuration duration)
        {
            _context.TourDurations.Update(duration);
            _context.SaveChanges();
            return duration;
        }

        // DELETE SINGLE
        public TourDuration Delete(Guid id)
        {
            var duration = _context.TourDurations
                .FirstOrDefault(td => td.Id == id);

            if (duration != null)
            {
                _context.TourDurations.Remove(duration);
                _context.SaveChanges();
            }

            return duration;
        }

        // DELETE BY TOUR ID
        public void DeleteByTourId(Guid tourId)
        {
            var durations = _context.TourDurations
                .Where(td => td.TourId == tourId)
                .ToList();

            _context.TourDurations.RemoveRange(durations);
            _context.SaveChanges();
        }

        // GET ALL
        public List<TourDuration> GetAll()
        {
            return _context.TourDurations
                .Include(td => td.Tour)
                .ToList();
        }
    }
}