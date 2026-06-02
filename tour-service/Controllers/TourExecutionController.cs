using Microsoft.AspNetCore.Mvc;
using tour_service.DTO;
using tour_service.Services;

namespace tour_service.Controllers
{
    [ApiController]
    [Route("tour-executions")]
    public class TourExecutionController : ControllerBase
    {
        private readonly TourExecutionService _service;

        public TourExecutionController(TourExecutionService service)
        {
            _service = service;
        }

        [HttpPost("start")]
        public IActionResult StartExecution([FromBody] StartTourExecutionRequest request)
        {
            try
            {
                var result = _service.StartExecution(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("check-position")]
        public IActionResult CheckPosition([FromBody] CheckPositionRequest request)
        {
            try
            {
                var result = _service.CheckPosition(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("{id}/complete")]
        public IActionResult CompleteExecution(Guid id)
        {
            try
            {
                var result = _service.CompleteExecution(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("{id}/abandon")]
        public IActionResult AbandonExecution(Guid id)
        {
            try
            {
                var result = _service.AbandonExecution(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            try
            {
                var result = _service.GetById(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }
    }
}
