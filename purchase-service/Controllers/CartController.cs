using Microsoft.AspNetCore.Mvc;
using purchase_service.Services;

namespace purchase_service.Controllers;

[ApiController]
public class CartController : ControllerBase
{
    private readonly PurchaseService _purchaseService;

    public CartController(PurchaseService purchaseService)
    {
        _purchaseService = purchaseService;
    }

    [HttpGet("cart")]
    public async Task<IActionResult> GetCart()
    {
        var touristId = ReadTouristId();
        if (touristId == null)
        {
            return BadRequest(new { error = "Invalid or missing X-User-ID header" });
        }

        return Ok(await _purchaseService.GetCart(touristId.Value));
    }

    [HttpPost("cart/items/{tourId}")]
    public async Task<IActionResult> AddToCart(Guid tourId)
    {
        var touristId = ReadTouristId();
        if (touristId == null)
        {
            return BadRequest(new { error = "Invalid or missing X-User-ID header" });
        }

        try
        {
            return Ok(await _purchaseService.AddToCart(touristId.Value, tourId));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("cart/items/{tourId}")]
    public async Task<IActionResult> RemoveFromCart(Guid tourId)
    {
        var touristId = ReadTouristId();
        if (touristId == null)
        {
            return BadRequest(new { error = "Invalid or missing X-User-ID header" });
        }

        return Ok(await _purchaseService.RemoveFromCart(touristId.Value, tourId));
    }

    [HttpPost("cart/checkout")]
    public async Task<IActionResult> Checkout()
    {
        var touristId = ReadTouristId();
        if (touristId == null)
        {
            return BadRequest(new { error = "Invalid or missing X-User-ID header" });
        }

        try
        {
            return Ok(await _purchaseService.Checkout(touristId.Value));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("purchases/{tourId}/token")]
    public async Task<IActionResult> GetToken(Guid tourId)
    {
        var touristId = ReadTouristId();
        if (touristId == null)
        {
            return BadRequest(new { error = "Invalid or missing X-User-ID header" });
        }

        var token = await _purchaseService.GetToken(touristId.Value, tourId);
        if (token == null)
        {
            return NotFound(new { error = "Purchase token not found" });
        }

        return Ok(token);
    }

    private int? ReadTouristId()
    {
        var userIdHeader = HttpContext.Request.Headers["X-User-ID"].ToString();
        return int.TryParse(userIdHeader, out var touristId) ? touristId : null;
    }
}
