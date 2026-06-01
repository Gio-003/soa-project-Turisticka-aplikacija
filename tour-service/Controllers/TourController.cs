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
        public IActionResult GetAllTours()
        {
            var tours = _tourService.GetAllTours();
            return Ok(tours);
        }

        [HttpGet("{id}")]
        public IActionResult GetTourById(Guid id)
        {
            var tour = _tourService.GetTourById(id);
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


    }
}