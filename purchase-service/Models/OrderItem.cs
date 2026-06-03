namespace purchase_service.Models;

public class OrderItem
{
    public Guid Id { get; set; }
    public Guid ShoppingCartId { get; set; }
    public ShoppingCart? ShoppingCart { get; set; }
    public Guid TourId { get; set; }
    public string TourName { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
