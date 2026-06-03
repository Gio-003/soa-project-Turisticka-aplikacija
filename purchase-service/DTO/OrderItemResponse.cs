namespace purchase_service.DTO;

public class OrderItemResponse
{
    public Guid Id { get; set; }
    public Guid TourId { get; set; }
    public string TourName { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
