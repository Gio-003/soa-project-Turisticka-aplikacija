package handlers

import (
	"encoding/json"
	"net/http"

	"github.com/gorilla/mux"
	"api-gateway/rpc"
)
//const tourServiceRPCURL = "http://tour-service:8080/rpc"
const tourServiceRPCURL = "http://localhost:55814/rpc"

//
// ==========================
// GET /api/tours/all
// ==========================
//
func GetAllToursRPC(w http.ResponseWriter, r *http.Request) {

	// 1. REST → JSON-RPC request
	rpcReq := rpc.ConvertRESTtoRPC(
		r,
		"GetAllTours",
		nil,
	)

	// 2. call .NET RPC service
	rpcResp, err := rpc.CallRPC(tourServiceRPCURL, rpcReq)
	if err != nil {
		http.Error(w, err.Error(), http.StatusInternalServerError)
		return
	}

	// 3. JSON-RPC → REST response
	result, statusCode := rpc.ConvertRPCtoREST(rpcResp)

	w.Header().Set("Content-Type", "application/json")
	w.WriteHeader(statusCode)
	json.NewEncoder(w).Encode(result)
}

//
// ==========================
// GET /api/tours/my/{authorId}
// ==========================
//
func GetMyToursRPC(w http.ResponseWriter, r *http.Request) {

	// 1. extract path param
	vars := mux.Vars(r)
	authorId := vars["authorId"]

	// 2. REST → JSON-RPC request
	rpcReq := rpc.ConvertRESTtoRPC(
		r,
		"GetMyTours",
		map[string]interface{}{
			"authorId": authorId,
		},
	)

	// 3. call .NET RPC service
	rpcResp, err := rpc.CallRPC(tourServiceRPCURL, rpcReq)
	if err != nil {
		http.Error(w, err.Error(), http.StatusInternalServerError)
		return
	}

	// 4. JSON-RPC → REST response
	result, statusCode := rpc.ConvertRPCtoREST(rpcResp)

	w.Header().Set("Content-Type", "application/json")
	w.WriteHeader(statusCode)
	json.NewEncoder(w).Encode(result)
}