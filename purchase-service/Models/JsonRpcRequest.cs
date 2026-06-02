using System.Text.Json;

namespace purchase_service.Models;

public class JsonRpcRequest
{
    public string jsonrpc { get; set; } = "2.0";
    public string method { get; set; } = string.Empty;
    public JsonElement @params { get; set; }
    public object? id { get; set; }
}
