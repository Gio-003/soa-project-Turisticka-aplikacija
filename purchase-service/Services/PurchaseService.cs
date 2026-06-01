using Microsoft.EntityFrameworkCore;
using purchase_service.Clients;
using purchase_service.Data;
using purchase_service.DTO;
using purchase_service.Models;

namespace purchase_service.Services;

public class PurchaseService
{
    private readonly AppDbContext _context;
    private readonly TourRpcClient _tourRpcClient;

    public PurchaseService(AppDbContext context, TourRpcClient tourRpcClient)
    {
        _context = context;
        _tourRpcClient = tourRpcClient;
    }

    public async Task<CartResponse> GetCart(int touristId)
    {
        var cart = await GetOrCreateCart(touristId);
        await NormalizeCart(cart);
        return ToCartResponse(cart);
    }

    public async Task<CartResponse> AddToCart(int touristId, Guid tourId)
    {
        if (await HasTourPurchaseToken(touristId, tourId))
        {
            throw new InvalidOperationException("Tour is already purchased.");
        }

        var tour = await _tourRpcClient.GetTourForPurchase(tourId);
        if (!string.Equals(tour.Status, "Published", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Only published tours can be added to cart.");
        }

        var cart = await GetOrCreateCart(touristId);
        if (cart.Items.Any(i => i.TourId == tourId))
        {
            await NormalizeCart(cart);
            return ToCartResponse(cart);
        }

        var item = new OrderItem
        {
            Id = Guid.NewGuid(),
            ShoppingCartId = cart.Id,
            TourId = tour.TourId,
            TourName = tour.TourName,
            Price = tour.Price
        };

        _context.OrderItems.Add(item);
        cart.Items.Add(item);

        RecalculateTotal(cart);
        await _context.SaveChangesAsync();

        return ToCartResponse(cart);
    }

    public async Task<CartResponse> RemoveFromCart(int touristId, Guid tourId)
    {
        var cart = await GetOrCreateCart(touristId);
        var item = cart.Items.FirstOrDefault(i => i.TourId == tourId);
        if (item != null)
        {
            _context.OrderItems.Remove(item);
            cart.Items.Remove(item);
            RecalculateTotal(cart);
            await _context.SaveChangesAsync();
        }

        return ToCartResponse(cart);
    }

    public async Task<CheckoutResponse> Checkout(int touristId)
    {
        var cart = await GetOrCreateCart(touristId);
        if (cart.Items.Count == 0)
        {
            throw new InvalidOperationException("Shopping cart is empty.");
        }

        var created = new List<TourPurchaseToken>();

        foreach (var item in cart.Items.ToList())
        {
            var existing = await _context.TourPurchaseTokens
                .FirstOrDefaultAsync(t => t.TouristId == touristId && t.TourId == item.TourId);

            if (existing != null)
            {
                continue;
            }

            var token = new TourPurchaseToken
            {
                Id = Guid.NewGuid(),
                TouristId = touristId,
                TourId = item.TourId,
                Token = Guid.NewGuid().ToString("N"),
                CreatedAt = DateTime.UtcNow
            };

            _context.TourPurchaseTokens.Add(token);
            created.Add(token);
        }

        _context.OrderItems.RemoveRange(cart.Items);
        cart.Items.Clear();
        RecalculateTotal(cart);
        await _context.SaveChangesAsync();

        return new CheckoutResponse
        {
            CreatedTokens = created.Count,
            Tokens = created.Select(ToTokenResponse).ToList()
        };
    }

    public async Task<bool> HasTourPurchaseToken(int touristId, Guid tourId)
    {
        return await _context.TourPurchaseTokens
            .AnyAsync(t => t.TouristId == touristId && t.TourId == tourId);
    }

    public async Task<TourPurchaseTokenResponse?> GetToken(int touristId, Guid tourId)
    {
        var token = await _context.TourPurchaseTokens
            .FirstOrDefaultAsync(t => t.TouristId == touristId && t.TourId == tourId);

        return token == null ? null : ToTokenResponse(token);
    }

    private async Task<ShoppingCart> GetOrCreateCart(int touristId)
    {
        var cart = await _context.ShoppingCarts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.TouristId == touristId);

        if (cart != null)
        {
            return cart;
        }

        cart = new ShoppingCart
        {
            Id = Guid.NewGuid(),
            TouristId = touristId,
            TotalPrice = 0
        };

        _context.ShoppingCarts.Add(cart);
        await _context.SaveChangesAsync();
        return cart;
    }

    private async Task NormalizeCart(ShoppingCart cart)
    {
        var duplicateItems = cart.Items
            .GroupBy(i => i.TourId)
            .SelectMany(group => group
                .OrderBy(i => i.Id)
                .Skip(1))
            .ToList();

        if (duplicateItems.Count > 0)
        {
            _context.OrderItems.RemoveRange(duplicateItems);
            foreach (var duplicate in duplicateItems)
            {
                cart.Items.Remove(duplicate);
            }
        }

        var previousTotal = cart.TotalPrice;
        RecalculateTotal(cart);

        if (duplicateItems.Count > 0 || previousTotal != cart.TotalPrice)
        {
            await _context.SaveChangesAsync();
        }
    }

    private static void RecalculateTotal(ShoppingCart cart)
    {
        cart.TotalPrice = cart.Items.Sum(i => i.Price);
    }

    private static CartResponse ToCartResponse(ShoppingCart cart)
    {
        return new CartResponse
        {
            Id = cart.Id,
            TouristId = cart.TouristId,
            TotalPrice = cart.Items.Sum(i => i.Price),
            Items = cart.Items.Select(i => new OrderItemResponse
            {
                Id = i.Id,
                TourId = i.TourId,
                TourName = i.TourName,
                Price = i.Price
            }).ToList()
        };
    }

    private static TourPurchaseTokenResponse ToTokenResponse(TourPurchaseToken token)
    {
        return new TourPurchaseTokenResponse
        {
            Id = token.Id,
            TouristId = token.TouristId,
            TourId = token.TourId,
            Token = token.Token,
            CreatedAt = token.CreatedAt
        };
    }
}
