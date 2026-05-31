package rpc

import (
	"bytes"
	"encoding/json"
	"net/http"
)

func CallRPC(url string, req *JSONRPCRequest) (*JSONRPCResponse, error) {

	body, err := json.Marshal(req)
	if err != nil {
		return nil, err
	}

	resp, err := http.Post(url, "application/json", bytes.NewBuffer(body))
	if err != nil {
		return nil, err
	}
	defer resp.Body.Close()

	var rpcResp JSONRPCResponse
	err = json.NewDecoder(resp.Body).Decode(&rpcResp)

	return &rpcResp, err
}