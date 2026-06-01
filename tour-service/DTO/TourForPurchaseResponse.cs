using tour_service.Enum;

namespace tour_service.DTO;

public class TourForPurchaseResponse
{
    public Guid TourId { get; set; }
    public string TourName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public TourStatus Status { get; set; }
}
