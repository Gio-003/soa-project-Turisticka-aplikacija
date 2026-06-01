namespace purchase_service.DTO;

public class TourForPurchaseResponse
{
    public Guid TourId { get; set; }
    public string TourName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Status { get; set; } = string.Empty;
}
