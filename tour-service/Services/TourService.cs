using Microsoft.EntityFrameworkCore;
using tour_service.Data;
using tour_service.DTO;
using tour_service.Enum;
using tour_service.Models;
using tour_service.Repositories;

namespace tour_service.Services
{
    public class TourService
    {
        private readonly AppDbContext _context;
        private readonly TourRepository _repository;

        public TourService(AppDbContext context, TourRepository tourRepository)
        {
            _context = context;
            _repository = tourRepository;
        }

        // CREATE TOUR
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
                Price = 0,

                // NOVO: inicijalno null
                PublishedAt = null,
                ArchivedAt = null,

                Tags = request.Tags != null
                    ? request.Tags.Select(tag => new TourTag
                    {
                        Name = tag
                    }).ToList()
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

        // PUBLISH TOUR
        public Tour PublishTour(Guid tourId)
        {
            var tour = _context.Tours.FirstOrDefault(t => t.Id == tourId);

            if (tour == null)
                throw new Exception("Tour not found");

            tour.Status = TourStatus.Published;
            tour.PublishedAt = DateTime.UtcNow;

            // ako se ponovo publishuje posle arhive
            tour.ArchivedAt = null;

            _context.SaveChanges();

            return tour;
        }

        // ARCHIVE TOUR
        public Tour ArchiveTour(Guid tourId)
        {
            var tour = _context.Tours.FirstOrDefault(t => t.Id == tourId);

            if (tour == null)
                throw new Exception("Tour not found");

            tour.Status = TourStatus.Archived;
            tour.ArchivedAt = DateTime.UtcNow;

            _context.SaveChanges();

            return tour;
        }

        // GET TOURS BY AUTHOR
        public List<TourResponse> GetToursByAuthor(int authorId)
        {
            return _context.Tours
                .Include(t => t.Tags)
                .Include(t => t.KeyPoints)
                .Where(t => t.AuthorId == authorId)
                .Select(t => new TourResponse
                {
                    Id = t.Id,
                    Name = t.Name,
                    Description = t.Description,
                    Difficulty = t.Difficulty,
                    Price = t.Price,
                    Status = t.Status,

                    Tags = t.Tags.Select(tag => tag.Name).ToList(),

                    KeyPoints = t.KeyPoints.Select(kp => new KeyPointResponse
                    {
                        Id = kp.Id,
                        Name = kp.Name,
                        Description = kp.Description,
                        ImageUrl = kp.ImageUrl,
                        Latitude = kp.Latitude,
                        Longitude = kp.Longitude
                    }).ToList(),

                    Durations = t.Durations.Select(d => new TourDurationResponse
                    {
                        TransportType = d.TransportType,
                        DurationInMinutes = d.DurationInMinutes
                    }).ToList(),
                })
                .ToList();
        }

        public List<Tour> GetAllTours()
        {
            return _repository.GetAll();
        }

        public Tour UpdateLength(Guid tourId, double length)
        {
            var tour = _context.Tours.FirstOrDefault(t => t.Id == tourId);

            if (tour == null)
                throw new Exception("Tour not found");

            tour.LengthInKm = length;

            _context.SaveChanges();

            return tour;
        }
    }
}