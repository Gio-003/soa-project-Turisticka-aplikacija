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

                Status = TourStatus.Draft,
                Price = 0,

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
                        ImageUrl = kp.Image, // 'image' sa fronta ide u 'ImageUrl' u bazi
                        Latitude = kp.Lat,   // 'lat' sa fronta ide u 'Latitude' u bazi
                        Longitude = kp.Lng   // 'lng' sa fronta ide u 'Longitude' u bazi
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

        // GET TOURS BY AUTHOR (RETURN DTO, NOT ENTITY)
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
    }
}