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
    }
}