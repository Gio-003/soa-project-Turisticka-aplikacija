using tour_service.Repositories;
using tour_service.Models;
using tour_service.DTO;
using System;

namespace tour_service.Services
{
    public class TourDurationService
    {
        private readonly TourDurationRepository _repository;

        public TourDurationService(TourDurationRepository repository)
        {
            _repository = repository;
        }

        // GET BY ID
        public TourDuration GetById(Guid id)
        {
            return _repository.GetById(id);
        }

        // GET BY TOUR ID
        public List<TourDuration> GetByTourId(Guid tourId)
        {
            return _repository.GetByTourId(tourId);
        }

        // ADD DURATION
        public TourDuration AddDuration(Guid tourId, CreateTourDurationDTO dto)
        {
            if (tourId == Guid.Empty)
                throw new ArgumentException("Invalid tour id");

            var duration = new TourDuration
            {
                Id = Guid.NewGuid(),
                TourId = tourId,
                TransportType = dto.TransportType,
                DurationInMinutes = dto.DurationInMinutes
            };

            return _repository.Create(duration);
        }

        // UPDATE DURATION
        public TourDuration UpdateDuration(Guid id, CreateTourDurationDTO dto)
        {
            var existing = _repository.GetById(id);

            if (existing == null)
                return null;

            existing.TransportType = dto.TransportType;
            existing.DurationInMinutes = dto.DurationInMinutes;

            return _repository.Update(existing);
        }

        // DELETE
        public TourDuration DeleteDuration(Guid id)
        {
            return _repository.Delete(id);
        }

        // DELETE BY TOUR
        public void DeleteByTourId(Guid tourId)
        {
            _repository.DeleteByTourId(tourId);
        }
    }
}