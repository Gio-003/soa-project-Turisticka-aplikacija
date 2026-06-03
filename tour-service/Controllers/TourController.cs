using Microsoft.AspNetCore.Mvc;
using tour_service.DTO;
using tour_service.Services;
using System;

namespace MyApp.Controllers
{
    [ApiController]
    [Route("tours")]
    public class TourController : ControllerBase
    {
        private readonly TourService _tourService;

        public TourController(TourService tourService)
        {
            _tourService = tourService;
        }

        [HttpPost]
        public IActionResult CreateTour([FromBody] CreateTourRequest request)
        {
            var tourResponse = _tourService.CreateTour(request);
            return Ok(tourResponse);
        }

        [HttpGet("my/{authorId}")]
        public IActionResult GetMyTours(int authorId)
        {
            var tours = _tourService.GetToursByAuthor(authorId);
            return Ok(tours);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllTours()
        {
            var tours = await _tourService.GetAllTours(ReadUserId(), ReadUserRole());
            return Ok(tours);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTourById(Guid id)
        {
            var tour = await _tourService.GetTourById(id, ReadUserId(), ReadUserRole());
            if (tour == null)
                return NotFound(new { error = "Tour not found" });
            return Ok(tour);
        }

        [HttpPost("{id}/publish")]
        public IActionResult PublishTour(Guid id)
        {
            var tour = _tourService.PublishTour(id);
            return Ok(tour);
        }

        [HttpPost("{id}/archive")]
        public IActionResult ArchiveTour(Guid id)
        {
            var tour = _tourService.ArchiveTour(id);
            return Ok(tour);
        }
        [HttpPut("{tourId}/length")]
        public IActionResult UpdateLength(Guid tourId,
         [FromBody] UpdateTourLength request)
        {
            var result = _tourService.UpdateLength(
                tourId,
                request.LengthInKm
            );

            return Ok(result);
        }

        [HttpPost("draft/{userId}")]
        public IActionResult CreateDraftTour(long userId)
        {
            var tourResponse = _tourService.CreateDraftTour(userId);
            return Ok(tourResponse);
        }

        [HttpDelete("{tourId}")]
        public IActionResult DeleteTour(Guid tourId)
        {
            _tourService.DeleteTour(tourId);
            return NoContent();
        }

        private int? ReadUserId()
        {
            var userIdHeader = HttpContext.Request.Headers["X-User-ID"].ToString();
            return int.TryParse(userIdHeader, out var userId) ? userId : null;
        }

        private string? ReadUserRole()
        {
            var role = HttpContext.Request.Headers["X-User-Role"].ToString();
            return string.IsNullOrWhiteSpace(role) ? null : role;
        }
    }
}
