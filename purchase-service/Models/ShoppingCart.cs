namespace purchase_service.Models;

public class ShoppingCart
{
    public Guid Id { get; set; }
    public int TouristId { get; set; }
    public decimal TotalPrice { get; set; }
    public List<OrderItem> Items { get; set; } = new();
}
