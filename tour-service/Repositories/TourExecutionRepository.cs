using Microsoft.EntityFrameworkCore;
using tour_service.Data;
using tour_service.Models;

namespace tour_service.Repositories
{
    public class TourExecutionRepository
    {
        private readonly AppDbContext _context;

        public TourExecutionRepository(AppDbContext context)
        {
            _context = context;
        }

        public TourExecution Create(TourExecution execution)
        {
            _context.TourExecutions.Add(execution);
            _context.SaveChanges();
            return execution;
        }

        public TourExecution? GetById(Guid id)
        {
            return _context.TourExecutions
                .Include(te => te.CompletedKeyPoints)
                .Include(te => te.Tour)
                    .ThenInclude(t => t.KeyPoints)
                .FirstOrDefault(te => te.Id == id);
        }

        public TourExecution? GetActiveByTouristAndTour(int touristId, Guid tourId)
        {
            return _context.TourExecutions
                .Include(te => te.CompletedKeyPoints)
                .Include(te => te.Tour)
                    .ThenInclude(t => t.KeyPoints)
                .FirstOrDefault(te =>
                    te.TouristId == touristId &&
                    te.TourId == tourId &&
                    te.Status == Enum.ExecutionStatus.Active);
        }

        public TourExecution Update(TourExecution execution)
        {
            _context.SaveChanges();
            return execution;
        }
    }
}
