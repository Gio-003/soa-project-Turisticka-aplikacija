package handlers

import (
	"api-gateway/config"
	"api-gateway/rpc"
	"encoding/json"
	"net/http"

	"github.com/gorilla/mux"
)

const tourServiceRPCURL = "http://tour-service:8080/rpc"
//const tourServiceRPCURL = "http://localhost:55816/rpc"
//const tourServiceRPCURL = "http://localhost:5001/rpc"

//



// ==========================
// GET /api/tours/all
// ==========================
func GetAllToursRPC(w http.ResponseWriter, r *http.Request) {
	rpcReq := rpc.ConvertRESTtoRPC(r, "GetAllTours", nil)

	rpcResp, err := rpc.CallRPC(config.ServiceURLs["tour"]+"/rpc", rpcReq)
	if err != nil {
		http.Error(w, err.Error(), http.StatusInternalServerError)
		return
	}

	result, statusCode := rpc.ConvertRPCtoREST(rpcResp)
	w.Header().Set("Content-Type", "application/json")
	w.WriteHeader(statusCode)
	json.NewEncoder(w).Encode(result)
}

// ==========================
// GET /api/tours/my/{authorId}
// ==========================
func GetMyToursRPC(w http.ResponseWriter, r *http.Request) {
	vars := mux.Vars(r)
	authorId := vars["authorId"]

	rpcReq := rpc.ConvertRESTtoRPC(r, "GetMyTours", map[string]interface{}{
		"authorId": authorId,
	})

	rpcResp, err := rpc.CallRPC(config.ServiceURLs["tour"]+"/rpc", rpcReq)
	if err != nil {
		http.Error(w, err.Error(), http.StatusInternalServerError)
		return
	}

	result, statusCode := rpc.ConvertRPCtoREST(rpcResp)
	w.Header().Set("Content-Type", "application/json")
	w.WriteHeader(statusCode)
	json.NewEncoder(w).Encode(result)
}

// ==========================
// POST /api/tours/{id}/publish
// ==========================
func PublishTourRPC(w http.ResponseWriter, r *http.Request) {
	vars := mux.Vars(r)
	tourId := vars["id"]

	rpcReq := rpc.ConvertRESTtoRPC(r, "PublishTour", map[string]interface{}{
		"tourId": tourId,
	})

	rpcResp, err := rpc.CallRPC(config.ServiceURLs["tour"]+"/rpc", rpcReq)
	if err != nil {
		http.Error(w, err.Error(), http.StatusInternalServerError)
		return
	}

	result, statusCode := rpc.ConvertRPCtoREST(rpcResp)
	w.Header().Set("Content-Type", "application/json")
	w.WriteHeader(statusCode)
	json.NewEncoder(w).Encode(result)
}

// ==========================
// POST /api/tours/{id}/archive
// ==========================
func ArchiveTourRPC(w http.ResponseWriter, r *http.Request) {
	vars := mux.Vars(r)
	tourId := vars["id"]

	rpcReq := rpc.ConvertRESTtoRPC(r, "ArchiveTour", map[string]interface{}{
		"tourId": tourId,
	})

	rpcResp, err := rpc.CallRPC(config.ServiceURLs["tour"]+"/rpc", rpcReq)
	if err != nil {
		http.Error(w, err.Error(), http.StatusInternalServerError)
		return
	}

	result, statusCode := rpc.ConvertRPCtoREST(rpcResp)
	w.Header().Set("Content-Type", "application/json")
	w.WriteHeader(statusCode)
	json.NewEncoder(w).Encode(result)
}