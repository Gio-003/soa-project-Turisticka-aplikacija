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
        [HttpPut("{id}")]
        public IActionResult UpdateKeyPoint([FromRoute] Guid tourId, [FromRoute] Guid id, [FromBody] CreateKeyPointDTO dto)
        {
            var updated = _service.UpdateExistingKeyPoint(id, dto);
            if (updated == null) return NotFound("Ključna tačka nije pronađena.");

            return Ok(updated);
        }

        // Dodat [FromRoute] radi eksplicitnog mapiranja sa frontenda
        [HttpDelete("{id}")]
        public IActionResult DeleteKeyPoint([FromRoute] Guid tourId, [FromRoute] Guid id)
        {
            var deleted = _service.DeleteKeyPoint(id);
            if (deleted == null) return NotFound("Ključna tačka nije pronađena.");

            return Ok(new { message = "Uspešno obrisano", id = id });
        }

    }
}
