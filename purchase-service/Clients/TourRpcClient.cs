using System.Text;
using System.Text.Json;
using purchase_service.DTO;

namespace purchase_service.Clients;

public class TourRpcClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public TourRpcClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<TourForPurchaseResponse> GetTourForPurchase(Guid tourId)
    {
        var rpcUrl = GetRpcUrl();
        var payload = new
        {
            jsonrpc = "2.0",
            method = "GetTourForPurchase",
            @params = new { tourId = tourId.ToString() },
            id = Guid.NewGuid().ToString()
        };

        using var content = new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8,
            "application/json");

        using var response = await _httpClient.PostAsync(rpcUrl, content);
        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync();
        using var document = await JsonDocument.ParseAsync(stream);
        var root = document.RootElement;

        if (root.TryGetProperty("error", out var error) && error.ValueKind != JsonValueKind.Null)
        {
            throw new InvalidOperationException(error.ToString());
        }

        var result = root.GetProperty("result");
        var tour = result.Deserialize<TourForPurchaseResponse>(new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (tour == null)
        {
            throw new InvalidOperationException("Tour service returned an empty purchase response.");
        }

        return tour;
    }

    private string GetRpcUrl()
    {
        return Environment.GetEnvironmentVariable("TOUR_RPC_URL")
            ?? _configuration["TourService:RpcUrl"]
            ?? "http://tour-service:8080/rpc";
    }
}
