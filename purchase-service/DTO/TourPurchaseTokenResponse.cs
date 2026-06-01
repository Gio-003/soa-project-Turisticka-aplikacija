namespace purchase_service.DTO;

public class TourPurchaseTokenResponse
{
    public Guid Id { get; set; }
    public int TouristId { get; set; }
    public Guid TourId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
