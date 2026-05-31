namespace tour_service.Models
{
    public class JsonRpcRequest
    {
        public string jsonrpc { get; set; }
        public string method { get; set; }
        public object @params { get; set; }
        public string id { get; set; }
    }
}
