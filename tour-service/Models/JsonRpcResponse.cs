namespace tour_service.Models
{
    public class JsonRpcResponse
    {
        public string jsonrpc { get; set; } = "2.0";
        public object result { get; set; }
        public object error { get; set; }
        public object? id { get; set; }
    }
}
