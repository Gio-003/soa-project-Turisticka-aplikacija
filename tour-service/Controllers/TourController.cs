using Microsoft.AspNetCore.Mvc;
using tour_service.DTO;
using tour_service.Services;

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
            int authorId = int.Parse(User.FindFirst("id")?.Value);

            var tourResponse = _tourService.CreateTour(request, authorId);

            return Ok(tourResponse);
        }

        [HttpGet("my")]
        public IActionResult GetMyTours()
        {
            int authorId = int.Parse(User.FindFirst("id")?.Value);

            var tours = _tourService.GetToursByAuthor(authorId);

            return Ok(tours);
        }
    }
}