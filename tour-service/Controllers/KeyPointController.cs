using Microsoft.AspNetCore.Mvc;
using tour_service.Services;
using tour_service.DTO;

namespace tour_service.Controllers
{
    [ApiController]
    [Route("tours/{tourId}/keypoints")]
    public class KeyPointController : Controller
    {
        private readonly KeyPointService _service;

        public KeyPointController(KeyPointService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult GetTourKeyPoints(Guid tourId)
        {
            var keyPoints = _service.GetByTourId(tourId);
            return Ok(keyPoints);
        }
        [HttpPost]
        public IActionResult AddKeyPoint([FromRoute]Guid tourId, [FromBody] CreateKeyPointDTO keyPointDTO)
        {
            var keyPoint = _service.AddKeyPoint(tourId, keyPointDTO);
            return Ok(keyPoint);
        }

    }
}
