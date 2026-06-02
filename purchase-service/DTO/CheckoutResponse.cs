namespace purchase_service.DTO;

public class CheckoutResponse
{
    public int CreatedTokens { get; set; }
    public List<TourPurchaseTokenResponse> Tokens { get; set; } = new();
}
