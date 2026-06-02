using System.Text;
using System.Text.Json;

namespace tour_service.Clients;

public class PurchaseRpcClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public PurchaseRpcClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<bool> HasTourPurchaseToken(int userId, Guid tourId)
    {
        var payload = new
        {
            jsonrpc = "2.0",
            method = "HasTourPurchaseToken",
            @params = new
            {
                userId = userId.ToString(),
                tourId = tourId.ToString()
            },
            id = Guid.NewGuid().ToString()
        };

        using var content = new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8,
            "application/json");

        try
        {
            using var response = await _httpClient.PostAsync(GetRpcUrl(), content);
            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            using var document = await JsonDocument.ParseAsync(stream);
            var root = document.RootElement;

            if (root.TryGetProperty("error", out var error) && error.ValueKind != JsonValueKind.Null)
            {
                return false;
            }

            return root.GetProperty("result").GetProperty("hasToken").GetBoolean();
        }
        catch
        {
            return false;
        }
    }

    private string GetRpcUrl()
    {
        return Environment.GetEnvironmentVariable("PURCHASE_RPC_URL")
            ?? _configuration["PurchaseService:RpcUrl"]
            ?? "http://purchase-service:8080/rpc";
    }
}
