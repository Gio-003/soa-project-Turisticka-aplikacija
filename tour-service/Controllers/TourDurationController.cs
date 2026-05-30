using Microsoft.AspNetCore.Mvc;
using tour_service.Services;
using tour_service.DTO;

namespace tour_service.Controllers
{
    [ApiController]
    [Route("api/tours/{tourId}/durations")]
    public class TourDurationController : Controller
    {
        private readonly TourDurationService _service;

        public TourDurationController(TourDurationService service)
        {
            _service = service;
        }

        // GET ALL DURATIONS FOR TOUR
        [HttpGet]
        public IActionResult GetTourDurations([FromRoute] Guid tourId)
        {
            var durations = _service.GetByTourId(tourId);
            return Ok(durations);
        }

        // ADD DURATION
        [HttpPost]
        public IActionResult AddDuration(
            [FromRoute] Guid tourId,
            [FromBody] CreateTourDurationDTO dto)
        {
            var duration = _service.AddDuration(tourId, dto);
            return Ok(duration);
        }

        // UPDATE DURATION
        [HttpPut("{id}")]
        public IActionResult UpdateDuration(
            [FromRoute] Guid tourId,
            [FromRoute] Guid id,
            [FromBody] CreateTourDurationDTO dto)
        {
            var updated = _service.UpdateDuration(id, dto);

            if (updated == null)
                return NotFound("Duration not found");

            return Ok(updated);
        }

        // DELETE DURATION
        [HttpDelete("{id}")]
        public IActionResult DeleteDuration(
            [FromRoute] Guid tourId,
            [FromRoute] Guid id)
        {
            var deleted = _service.DeleteDuration(id);

            if (deleted == null)
                return NotFound("Duration not found");

            return Ok(new
            {
                message = "Successfully deleted",
                id = id
            });
        }
    }
}