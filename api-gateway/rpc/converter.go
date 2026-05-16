package rpc

import (
	"encoding/json"
	"net/http"
)

// JSONRPCRequest represents a JSON-RPC 2.0 request
type JSONRPCRequest struct {
	JSONRPC string      `json:"jsonrpc"`
	Method  string      `json:"method"`
	Params  interface{} `json:"params"`
	ID      interface{} `json:"id"`
}

// JSONRPCResponse represents a JSON-RPC 2.0 response
type JSONRPCResponse struct {
	JSONRPC string      `json:"jsonrpc"`
	Result  interface{} `json:"result,omitempty"`
	Error   *RPCError   `json:"error,omitempty"`
	ID      interface{} `json:"id"`
}

// RPCError represents a JSON-RPC error
type RPCError struct {
	Code    int         `json:"code"`
	Message string      `json:"message"`
	Data    interface{} `json:"data,omitempty"`
}

// ConvertRESTtoRPC converts a REST request to JSON-RPC format
// This is a placeholder for future RPC implementation
func ConvertRESTtoRPC(r *http.Request, method string, params interface{}) *JSONRPCRequest {
	return &JSONRPCRequest{
		JSONRPC: "2.0",
		Method:  method,
		Params:  params,
		ID:      r.Header.Get("X-Request-ID"),
	}
}

// ConvertRPCtoREST converts a JSON-RPC response to REST format
// This is a placeholder for future RPC implementation
func ConvertRPCtoREST(rpcResp *JSONRPCResponse) (interface{}, int) {
	if rpcResp.Error != nil {
		return map[string]interface{}{
			"error": rpcResp.Error.Message,
			"code":  rpcResp.Error.Code,
		}, 400
	}
	return rpcResp.Result, 200
}

// ParseJSONRPCRequest parses a JSON-RPC request from raw data
func ParseJSONRPCRequest(data []byte) (*JSONRPCRequest, error) {
	var req JSONRPCRequest
	err := json.Unmarshal(data, &req)
	return &req, err
}

// MarshalJSONRPCResponse marshals a JSON-RPC response to JSON
func MarshalJSONRPCResponse(resp *JSONRPCResponse) ([]byte, error) {
	return json.Marshal(resp)
}
