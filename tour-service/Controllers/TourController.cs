using Microsoft.AspNetCore.Mvc;
using tour_service.DTO;
using tour_service.Services;
using System;

namespace MyApp.Controllers
{
    [ApiController]
    [Route("api/tours")]
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
            /*var userIdClaim = User.FindFirst("id")?.Value;

            if (!Guid.TryParse(userIdClaim, out Guid authorId))
            {
                return Unauthorized("Invalid or missing user id in token.");
            }*/
            Guid id = Guid.NewGuid();
            var tourResponse = _tourService.CreateTour(request, id);

            return Ok(tourResponse);
        }

        [HttpGet("my")]
        public IActionResult GetMyTours()
        {
            var userIdClaim = User.FindFirst("id")?.Value;

            if (!Guid.TryParse(userIdClaim, out Guid authorId))
            {
                return Unauthorized("Invalid or missing user id in token.");
            }

            var tours = _tourService.GetToursByAuthor(authorId);

            return Ok(tours);
        }
        [HttpGet("all")]
        public IActionResult GetAllTours()
        {
            var tours = _tourService.GetAllTours();
            return Ok(tours);
        }
    }
}