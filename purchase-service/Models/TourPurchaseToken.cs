namespace purchase_service.Models;

public class TourPurchaseToken
{
    public Guid Id { get; set; }
    public int TouristId { get; set; }
    public Guid TourId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
