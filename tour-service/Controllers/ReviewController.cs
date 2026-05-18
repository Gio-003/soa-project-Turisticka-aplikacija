using Microsoft.AspNetCore.Mvc;
using tour_service.DTO;
using tour_service.Services;

namespace tour_service.Controllers
{
    [ApiController]
    [Route("api/tours/{tourId}/reviews")]
    public class ReviewController : ControllerBase
    {
        private readonly ReviewService _reviewService;

        public ReviewController(ReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        // GET /api/tours/{tourId}/reviews - Sve recenzije za turu
        [HttpGet]
        public IActionResult GetReviewsByTour(Guid tourId)
        {
            var reviews = _reviewService.GetReviewsByTourId(tourId);
            return Ok(reviews);
        }

        // GET /api/tours/{tourId}/reviews/stats - Statistika ocena
        [HttpGet("stats")]
        public IActionResult GetReviewStats(Guid tourId)
        {
            var count = _reviewService.GetReviewCount(tourId);
            var average = _reviewService.GetAverageRating(tourId);
            return Ok(new { count, averageRating = Math.Round(average, 2) });
        }

        // POST /api/tours/{tourId}/reviews - Dodaj novu recenziju
        [HttpPost]
        public IActionResult CreateReview(Guid tourId, [FromBody] CreateReviewDTO request)
        {
            var userIdClaim = User.FindFirst("id")?.Value;
            var userNameClaim = User.FindFirst("given_name")?.Value ?? "Anonymous";
            var userPictureClaim = User.FindFirst("picture")?.Value ?? "";

            if (!Guid.TryParse(userIdClaim, out Guid userId))
            {
                return Unauthorized("Invalid or missing user id in token");
            }

            try
            {
                var review = _reviewService.CreateReview(tourId, userId, request, userNameClaim, userPictureClaim);
                return CreatedAtAction(nameof(GetReviewById), new { reviewId = review.Id }, review);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // GET /api/reviews/{reviewId} - Jedna recenzija
        [HttpGet("/{reviewId}")]
        public IActionResult GetReviewById(Guid reviewId)
        {
            try
            {
                var review = _reviewService.GetReviewById(reviewId);
                return Ok(review);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { error = "Review not found" });
            }
        }

        // PUT /api/reviews/{reviewId} - Ažuriranje recenzije
        [HttpPut("/{reviewId}")]
        public IActionResult UpdateReview(Guid reviewId, [FromBody] CreateReviewDTO request)
        {
            var userIdClaim = User.FindFirst("id")?.Value;

            if (!Guid.TryParse(userIdClaim, out Guid userId))
            {
                return Unauthorized("Invalid or missing user id in token");
            }

            try
            {
                var review = _reviewService.UpdateReview(reviewId, request, userId);
                return Ok(review);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { error = "Review not found" });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // DELETE /api/reviews/{reviewId} - Brisanje recenzije
        [HttpDelete("/{reviewId}")]
        public IActionResult DeleteReview(Guid reviewId)
        {
            var userIdClaim = User.FindFirst("id")?.Value;

            if (!Guid.TryParse(userIdClaim, out Guid userId))
            {
                return Unauthorized("Invalid or missing user id in token");
            }

            try
            {
                _reviewService.DeleteReview(reviewId, userId);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { error = "Review not found" });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }
    }
}
