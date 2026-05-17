using Microsoft.EntityFrameworkCore;
using tour_service.Data;
using tour_service.DTO;
using tour_service.Enum;
using tour_service.Models;

namespace tour_service.Services
{
    public class TourService
    {
        private readonly AppDbContext _context;

        public TourService(AppDbContext context)
        {
            _context = context;
        }

        // CREATE TOUR
        public Tour CreateTour(CreateTourRequest request, Guid authorId)
        {
            var tour = new Tour
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                Difficulty = request.Difficulty,

                AuthorId = authorId,

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
                    : new List<KeyPoints>()
            };

        _context.Tours.Add(tour);
            _context.SaveChanges();

            return tour;
        }

        // GET TOURS BY AUTHOR (RETURN DTO, NOT ENTITY)
        public List<TourResponse> GetToursByAuthor(Guid authorId)
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
                    AuthorId = t.AuthorId,
                    Tags = t.Tags.Select(tag => tag.Name).ToList()
                })
                .ToList();
        }
    }
}