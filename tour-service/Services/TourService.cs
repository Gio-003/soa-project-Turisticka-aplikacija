using Microsoft.EntityFrameworkCore;
using tour_service.Clients;
using tour_service.Data;
using tour_service.DTO;
using tour_service.Enum;
using tour_service.Models;
using tour_service.Repositories;
using tour_service.Saga;

namespace tour_service.Services
{
    public class TourService
    {
        private readonly AppDbContext _context;
        private readonly TourRepository _repository;
        private readonly PurchaseRpcClient _purchaseRpcClient;
        private readonly PublishTourOrchestrator _orchestrator;

        public TourService(
            AppDbContext context,
            TourRepository tourRepository,
            PurchaseRpcClient purchaseRpcClient,
            PublishTourOrchestrator orchestrator)
        {
            _context = context;
            _repository = tourRepository;
            _purchaseRpcClient = purchaseRpcClient;
            _orchestrator = orchestrator;
        }

        public Tour CreateTour(CreateTourRequest request)
        {
            var tour = new Tour
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                Difficulty = request.Difficulty,
                AuthorId = request.AuthorId,
                LengthInKm = request.LengthInKm,
                Status = TourStatus.Draft,
                Price = request.Price,
                PublishedAt = null,
                ArchivedAt = null,
                Tags = request.Tags != null
                    ? request.Tags.Select(tag => new TourTag { Name = tag }).ToList()
                    : new List<TourTag>(),
                KeyPoints = request.KeyPoints != null
                    ? request.KeyPoints.Select(kp => new KeyPoints
                    {
                        Id = Guid.NewGuid(),
                        Name = kp.Name,
                        Description = kp.Description,
                        ImageUrl = kp.Image,
                        Latitude = kp.Lat,
                        Longitude = kp.Lng
                    }).ToList()
                    : new List<KeyPoints>(),
                Durations = request.Durations != null
                    ? request.Durations.Select(d => new TourDuration
                    {
                        Id = Guid.NewGuid(),
                        TransportType = d.TransportType,
                        DurationInMinutes = d.DurationInMinutes
                    }).ToList()
                    : new List<TourDuration>(),
            };

            _context.Tours.Add(tour);
            _context.SaveChanges();

            return tour;
        }

        public Guid CreateDraftTour(long userId)
        {
            var tour = new Tour
            {
                Id = Guid.NewGuid(),
                Name = "My first tour",
                Description = "This is my first tour",
                Difficulty = "EASY", // ili neki default enum/int
                AuthorId = (int)userId,
                LengthInKm = 0,

                Status = TourStatus.Draft,
                Price = 0,

                PublishedAt = null,
                ArchivedAt = null,

                Tags = new List<TourTag>(),

                KeyPoints = new List<KeyPoints>(),

                Durations = new List<TourDuration>()
            };

            _context.Tours.Add(tour);
            _context.SaveChanges();

            return tour.Id;
        }

        public void DeleteTour(Guid tourId)
        {
            var tour = _context.Tours
                .FirstOrDefault(t => t.Id == tourId);

            if (tour == null)
            {
                throw new Exception("Tour not found");
            }

            _context.Tours.Remove(tour);
            _context.SaveChanges();
        }

        // PUBLISH TOUR
        public Tour PublishTour(Guid tourId)
        {
            var tour = _context.Tours.FirstOrDefault(t => t.Id == tourId);
            if (tour == null)
            {
                throw new Exception("Tour not found");
            }

            tour.Status = TourStatus.Published;
            tour.PublishedAt = DateTime.UtcNow;
            tour.ArchivedAt = null;

            _context.SaveChanges();
            _orchestrator.Start(tour);

            return tour;
        }
        public void RollbackTour(Guid tourId)
        {
            var tour = _context.Tours.FirstOrDefault(t => t.Id == tourId);
            if (tour != null)
            {
                tour.Status = TourStatus.Draft;
                tour.PublishedAt = null;
                _context.SaveChanges();
            }
        }

        public Tour ArchiveTour(Guid tourId)
        {
            var tour = _context.Tours.FirstOrDefault(t => t.Id == tourId);
            if (tour == null)
            {
                throw new Exception("Tour not found");
            }

            tour.Status = TourStatus.Archived;
            tour.ArchivedAt = DateTime.UtcNow;

            _context.SaveChanges();

            return tour;
        }

        public List<TourResponse> GetToursByAuthor(int authorId)
        {
            return _context.Tours
                .Include(t => t.Tags)
                .Include(t => t.KeyPoints)
                .Include(t => t.Durations)
                .Where(t => t.AuthorId == authorId)
                .AsEnumerable()
                .Select(t => ToTourResponse(t, true, true))
                .ToList();
        }

        public async Task<List<TourResponse>> GetAllTours(int? userId, string? role)
        {
            var tours = _repository.GetAll();
            var result = new List<TourResponse>();

            foreach (var tour in tours)
            {
                var includeAll = await ShouldIncludeAllKeyPoints(tour, userId, role);
                result.Add(ToTourResponse(tour, includeAll, includeAll));
            }

            return result;
        }

        public List<TourResponse> GetAllTours()
        {
            return _repository.GetAll()
                .Select(t => ToTourResponse(t, false, false))
                .ToList();
        }

        public Tour UpdateLength(Guid tourId, double length)
        {
            var tour = _context.Tours.FirstOrDefault(t => t.Id == tourId);
            if (tour == null)
            {
                throw new Exception("Tour not found");
            }

            tour.LengthInKm = length;
            _context.SaveChanges();

            return tour;
        }

        public async Task<TourResponse?> GetTourById(Guid tourId, int? userId, string? role)
        {
            var tour = _repository.GetById(tourId);
            if (tour == null)
            {
                return null;
            }

            var includeAll = await ShouldIncludeAllKeyPoints(tour, userId, role);
            return ToTourResponse(tour, includeAll, includeAll);
        }

        public TourForPurchaseResponse GetTourForPurchase(Guid tourId)
        {
            var tour = _repository.GetById(tourId);
            if (tour == null)
            {
                throw new Exception("Tour not found");
            }

            return new TourForPurchaseResponse
            {
                TourId = tour.Id,
                TourName = tour.Name,
                Price = tour.Price,
                Status = tour.Status
            };
        }

        private async Task<bool> ShouldIncludeAllKeyPoints(Tour tour, int? userId, string? role)
        {
            if (string.Equals(role, "GUIDE", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(role, "ADMIN", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (userId.HasValue && tour.AuthorId == userId.Value)
            {
                return true;
            }

            if (!userId.HasValue)
            {
                return false;
            }

            return await _purchaseRpcClient.HasTourPurchaseToken(userId.Value, tour.Id);
        }

        private static TourResponse ToTourResponse(Tour tour, bool includeAllKeyPoints, bool isPurchased)
        {
            var keyPoints = includeAllKeyPoints
                ? tour.KeyPoints
                : tour.KeyPoints.Take(1);

            return new TourResponse
            {
                Id = tour.Id,
                Name = tour.Name,
                Description = tour.Description,
                Difficulty = tour.Difficulty,
                Price = tour.Price,
                Status = tour.Status,
                Tags = tour.Tags.Select(tag => tag.Name).ToList(),
                KeyPoints = keyPoints.Select(kp => new KeyPointResponse
                {
                    Id = kp.Id,
                    Name = kp.Name,
                    Description = kp.Description,
                    ImageUrl = kp.ImageUrl,
                    Latitude = kp.Latitude,
                    Longitude = kp.Longitude
                }).ToList(),
                Durations = tour.Durations.Select(d => new TourDurationResponse
                {
                    TransportType = d.TransportType,
                    DurationInMinutes = d.DurationInMinutes
                }).ToList(),
                LengthInKm = tour.LengthInKm,
                IsPurchased = isPurchased
            };
        }
    }
}
