using tour_service.Repositories;
using tour_service.Models;
using tour_service.DTO;

namespace tour_service.Services
{
    public class KeyPointService
    {
        private readonly KeyPointRepository _repository;
        public KeyPointService(KeyPointRepository repository)
        {
            _repository = repository;
        }

        public KeyPoints GetById(Guid id)
        {
            return _repository.GetById(id);
        }
        public List<KeyPoints> GetByTourId(Guid tourId)
            {
                return _repository.GetByTourId(tourId);
        }

        public KeyPoints AddKeyPoint(Guid tourdId,CreateKeyPointDTO keyPointDTO)
        {
            if (tourdId == Guid.Empty)
                throw new ArgumentException("Invalid tour id");
            var keyPoint = new KeyPoints
            {
                Id = Guid.NewGuid(),
                Name = keyPointDTO.Name,
                Description = keyPointDTO.Description,
                ImageUrl = keyPointDTO.ImageUrl,
                TourId = tourdId,
                Longitude = keyPointDTO.Longitude,
                Latitude = keyPointDTO.Latitude
            };
            return _repository.Create(keyPoint);
        }
         public KeyPoints Update(KeyPoints keyPoint)
        {
            return _repository.Update(keyPoint);
        }
    }
}
