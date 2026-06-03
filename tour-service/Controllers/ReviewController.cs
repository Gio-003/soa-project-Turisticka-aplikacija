using Microsoft.AspNetCore.Mvc;
using tour_service.Services;
using tour_service.DTO;

namespace tour_service.Controllers
{
    [ApiController]
    [Route("tours/{tourId}/reviews")]
    public class ReviewController : ControllerBase
    {
        private readonly ReviewService _service;

        public ReviewController(ReviewService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult GetTourReviews(Guid tourId)
        {
            var reviews = _service.GetReviewsByTourId(tourId);
            return Ok(reviews);
        }

        [HttpPost]
        public async Task<IActionResult> CreateReview([FromRoute] Guid tourId, [FromBody] CreateReviewRequest request)
        {
            var userIdHeader = HttpContext.Request.Headers["X-User-ID"].ToString();
            var userRoleHeader = HttpContext.Request.Headers["X-User-Role"].ToString();
            var username = HttpContext.Request.Headers["X-Username"].ToString();

            if (!int.TryParse(userIdHeader, out int touristId))
            {
                return BadRequest(new { error = "Invalid or missing X-User-ID header" });
            }

            if (string.IsNullOrEmpty(userRoleHeader))
            {
                return BadRequest(new { error = "Invalid or missing X-User-Role header" });
            }

            if (string.IsNullOrEmpty(username))
            {
                return BadRequest(new { error = "Invalid or missing X-Username header" });
            }

            if (!string.Equals(userRoleHeader, "TOURIST", StringComparison.OrdinalIgnoreCase))
            {
                return Forbid();
            }

            try
            {
                var review = await _service.CreateReview(tourId, touristId, username, request);
                return Ok(review);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { error = ex.Message });
            }
        }
    }
}
