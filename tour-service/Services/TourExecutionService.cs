using Microsoft.EntityFrameworkCore;
using tour_service.Data;
using tour_service.DTO;
using tour_service.Enum;
using tour_service.Models;
using tour_service.Repositories;

namespace tour_service.Services
{
    public class TourExecutionService
    {
        private readonly TourExecutionRepository _repository;
        private readonly AppDbContext _context;

        public TourExecutionService(TourExecutionRepository repository, AppDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public TourExecutionResponse StartExecution(StartTourExecutionRequest request)
        {
            var tour = _context.Tours
                .Include(t => t.KeyPoints)
                .FirstOrDefault(t => t.Id == request.TourId);

            if (tour == null)
                throw new Exception("Tour not found");

            if (tour.Status != TourStatus.Published && tour.Status != TourStatus.Archived)
                throw new Exception("Tour is not available for execution");

            var existing = _repository.GetActiveByTouristAndTour(request.TouristId, request.TourId);
            if (existing != null)
                throw new Exception("Tourist already has an active execution for this tour");

            var execution = new TourExecution
            {
                Id = Guid.NewGuid(),
                TourId = request.TourId,
                TouristId = request.TouristId,
                Status = ExecutionStatus.Active,
                StartedAt = DateTime.UtcNow,
                LastActivity = DateTime.UtcNow,
                StartLatitude = request.Latitude,
                StartLongitude = request.Longitude,
                CompletedKeyPoints = new List<CompletedKeyPoint>()
            };

            _repository.Create(execution);
            return MapToResponse(execution);
        }

        public CheckPositionResponse CheckPosition(CheckPositionRequest request)
        {
            var execution = _repository.GetById(request.TourExecutionId);
            if (execution == null)
                throw new Exception("Execution not found");

            execution.LastActivity = DateTime.UtcNow;

            var alreadyCompletedIds = execution.CompletedKeyPoints
                .Select(ckp => ckp.KeyPointId)
                .ToHashSet();

            var remainingKeyPoints = execution.Tour.KeyPoints
                .Where(kp => !alreadyCompletedIds.Contains(kp.Id))
                .ToList();

            KeyPoints? nearbyKeyPoint = null;
            foreach (var kp in remainingKeyPoints)
            {
                var distance = CalculateDistance(request.Latitude, request.Longitude, kp.Latitude, kp.Longitude);
                if (distance <= 200)
                {
                    nearbyKeyPoint = kp;
                    break;
                }
            }

            bool completed = false;
            if (nearbyKeyPoint != null)
            {
                var completedKp = new CompletedKeyPoint
                {
                    Id = Guid.NewGuid(),
                    TourExecutionId = execution.Id,
                    KeyPointId = nearbyKeyPoint.Id,
                    CompletedAt = DateTime.UtcNow
                };
                _context.CompletedKeyPoints.Add(completedKp);
                execution.CompletedKeyPoints.Add(completedKp);
                completed = true;
            }

            _repository.Update(execution);

            return new CheckPositionResponse
            {
                NearbyKeyPointId = nearbyKeyPoint?.Id,
                NearbyKeyPointName = nearbyKeyPoint?.Name,
                KeyPointCompleted = completed,
                LastActivity = execution.LastActivity
            };
        }

        public TourExecutionResponse CompleteExecution(Guid executionId)
        {
            var execution = _repository.GetById(executionId);
            if (execution == null)
                throw new Exception("Execution not found");

            execution.Status = ExecutionStatus.Completed;
            execution.CompletedAt = DateTime.UtcNow;
            execution.LastActivity = DateTime.UtcNow;

            _repository.Update(execution);
            return MapToResponse(execution);
        }

        public TourExecutionResponse AbandonExecution(Guid executionId)
        {
            var execution = _repository.GetById(executionId);
            if (execution == null)
                throw new Exception("Execution not found");

            execution.Status = ExecutionStatus.Abandoned;
            execution.AbandonedAt = DateTime.UtcNow;
            execution.LastActivity = DateTime.UtcNow;

            _repository.Update(execution);
            return MapToResponse(execution);
        }

        public TourExecutionResponse GetById(Guid executionId)
        {
            var execution = _repository.GetById(executionId);
            if (execution == null)
                throw new Exception("Execution not found");
            return MapToResponse(execution);
        }

        private TourExecutionResponse MapToResponse(TourExecution execution)
        {
            return new TourExecutionResponse
            {
                Id = execution.Id,
                TourId = execution.TourId,
                TouristId = execution.TouristId,
                Status = execution.Status.ToString(),
                StartedAt = execution.StartedAt,
                CompletedAt = execution.CompletedAt,
                AbandonedAt = execution.AbandonedAt,
                LastActivity = execution.LastActivity,
                CompletedKeyPoints = execution.CompletedKeyPoints.Select(ckp => new CompletedKeyPointDto
                {
                    KeyPointId = ckp.KeyPointId,
                    CompletedAt = ckp.CompletedAt
                }).ToList()
            };
        }

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371000;
            var dLat = (lat2 - lat1) * Math.PI / 180;
            var dLon = (lon2 - lon1) * Math.PI / 180;
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }
    }
}
