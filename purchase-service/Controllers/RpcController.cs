using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using purchase_service.Models;
using purchase_service.Services;

namespace purchase_service.Controllers;

[ApiController]
[Route("rpc")]
public class RpcController : ControllerBase
{
    private readonly PurchaseService _purchaseService;

    public RpcController(PurchaseService purchaseService)
    {
        _purchaseService = purchaseService;
    }

    [HttpPost]
    public async Task<IActionResult> Handle([FromBody] JsonElement body)
    {
        var request = JsonSerializer.Deserialize<JsonRpcRequest>(body);
        var response = new JsonRpcResponse
        {
            jsonrpc = "2.0",
            id = request?.id
        };

        try
        {
            if (request == null)
            {
                throw new InvalidOperationException("Invalid JSON-RPC request.");
            }

            switch (request.method)
            {
                case "HasTourPurchaseToken":
                    var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(
                        request.@params.ToString()
                    ) ?? new Dictionary<string, string>();

                    var userId = int.Parse(dict["userId"]);
                    var tourId = Guid.Parse(dict["tourId"]);
                    response.result = new
                    {
                        hasToken = await _purchaseService.HasTourPurchaseToken(userId, tourId)
                    };
                    break;

                default:
                    response.error = new
                    {
                        code = -32601,
                        message = "Method not found"
                    };
                    break;
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            response.error = new
            {
                code = -32603,
                message = ex.Message
            };

            return Ok(response);
        }
    }
}
