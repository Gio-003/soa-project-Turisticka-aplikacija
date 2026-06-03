namespace tour_service.Models
{
    public class JsonRpcRequest
    {
        public string jsonrpc { get; set; } = "2.0";
        public string method { get; set; } = string.Empty;
        public object @params { get; set; }
        public object? id { get; set; }
    }
}
