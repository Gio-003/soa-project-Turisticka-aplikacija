namespace purchase_service.DTO;

public class CartResponse
{
    public Guid Id { get; set; }
    public int TouristId { get; set; }
    public decimal TotalPrice { get; set; }
    public List<OrderItemResponse> Items { get; set; } = new();
}
