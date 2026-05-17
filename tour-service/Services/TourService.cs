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
        public Tour CreateTour(CreateTourRequest request, int authorId)
        {
            var tour = new Tour
            {
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
                    : new List<TourTag>()
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