using tour_service.Models;
using tour_service.Data;

namespace tour_service.Repositories
{
    public class KeyPointRepository
    {
        private readonly AppDbContext _context;

        public KeyPointRepository(AppDbContext context)
        {
            _context = context;
        }

        public KeyPoints Create(KeyPoints keyPoint)
        {
            _context.KeyPoints.Add(keyPoint);
            _context.SaveChanges();
            return keyPoint;
        }

        public KeyPoints GetById(Guid id)
        {
            return _context.KeyPoints.Find(id);
        }

        public KeyPoints Update(KeyPoints keyPoint)
        {
            _context.KeyPoints.Update(keyPoint);
            _context.SaveChanges();
            return keyPoint;
        }
        public KeyPoints Delete(Guid id)
        {
            var keyPoint = _context.KeyPoints.Find(id);
            if (keyPoint != null)
            {
                _context.KeyPoints.Remove(keyPoint);
                _context.SaveChanges();
            }
            return keyPoint;

        }
        public List<KeyPoints> GetAll()
        {
            return _context.KeyPoints.ToList();
        }
        public List<KeyPoints>  GetByTourId(Guid tourId)
        {
            return _context.KeyPoints.Where(kp => kp.TourId == tourId).ToList();
        }

    }
}
