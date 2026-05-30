package router

import (
	pb "api-gateway/proto"
	"context"
	"encoding/json"
	"net/http"
	"time"

	"github.com/gorilla/mux"

	grpcclient "api-gateway/grpc"
)

func GetAllBlogsGrpc(w http.ResponseWriter, r *http.Request) {
	ctx, cancel := context.WithTimeout(context.Background(), 5*time.Second)
	defer cancel()

	// Direktno koristiš globalnog klijenta
	resp, err := grpcclient.BlogClient.GetAllBlogs(ctx, &pb.EmptyRequest{})
	if err != nil {
		http.Error(w, "RPC error: "+err.Error(), http.StatusInternalServerError)
		return
	}

	w.Header().Set("Content-Type", "application/json")
	json.NewEncoder(w).Encode(resp.Blogs)
}
func GetBlogByIDGrpc(
	w http.ResponseWriter,
	r *http.Request,
) {

	vars := mux.Vars(r)

	id := vars["id"]

	resp, err := grpcclient.GetBlogByIDRPC(id)

	if err != nil {
		http.Error(
			w,
			err.Error(),
			http.StatusInternalServerError,
		)
		return
	}

	w.Header().Set(
		"Content-Type",
		"application/json",
	)

	json.NewEncoder(w).Encode(resp.Blog)
}
