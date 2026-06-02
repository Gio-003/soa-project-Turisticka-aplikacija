package router

import (
	"api-gateway/config"
	grpcclient "api-gateway/grpc"
	pb "api-gateway/proto"
	"context"
	"encoding/json"
	"fmt"
	"io"
	"net/http"
	"time"

	"github.com/gorilla/mux"
)

type followerPermissionResponse struct {
	CanRead    bool `json:"canRead"`
	CanComment bool `json:"canComment"`
}

type BlogAuthorResponse struct {
	ID        string `json:"id"`
	BlogCount int    `json:"blogCount"`
}

func GetAllBlogsGrpc(w http.ResponseWriter, r *http.Request) {
	ctx, cancel := context.WithTimeout(context.Background(), 5*time.Second)
	defer cancel()

	resp, err := grpcclient.BlogClient.GetAllBlogs(ctx, &pb.EmptyRequest{})
	if err != nil {
		http.Error(w, "RPC error: "+err.Error(), http.StatusInternalServerError)
		return
	}

	userID := r.Header.Get("X-User-ID")
	filteredBlogs := make([]*pb.BlogMessage, 0, len(resp.Blogs))
	for _, blog := range resp.Blogs {
		if blog.AuthorId == userID || followerPermission(r, userID, blog.AuthorId, "can-read") {
			filteredBlogs = append(filteredBlogs, blog)
		}
	}

	w.Header().Set("Content-Type", "application/json")
	json.NewEncoder(w).Encode(filteredBlogs)
}

func GetBlogAuthorsGrpc(w http.ResponseWriter, r *http.Request) {
	ctx, cancel := context.WithTimeout(context.Background(), 5*time.Second)
	defer cancel()

	resp, err := grpcclient.BlogClient.GetAllBlogs(ctx, &pb.EmptyRequest{})
	if err != nil {
		http.Error(w, "RPC error: "+err.Error(), http.StatusInternalServerError)
		return
	}

	userID := r.Header.Get("X-User-ID")
	authors := make(map[string]int)
	for _, blog := range resp.Blogs {
		if blog.AuthorId == "" || blog.AuthorId == userID || followerPermission(r, userID, blog.AuthorId, "can-read") {
			continue
		}
		authors[blog.AuthorId]++
	}

	result := make([]BlogAuthorResponse, 0, len(authors))
	for authorID, count := range authors {
		result = append(result, BlogAuthorResponse{
			ID:        authorID,
			BlogCount: count,
		})
	}

	w.Header().Set("Content-Type", "application/json")
	json.NewEncoder(w).Encode(result)
}

func GetBlogByIDGrpc(w http.ResponseWriter, r *http.Request) {
	vars := mux.Vars(r)
	id := vars["id"]

	resp, err := grpcclient.GetBlogByIDRPC(id)
	if err != nil {
		http.Error(w, err.Error(), http.StatusInternalServerError)
		return
	}

	userID := r.Header.Get("X-User-ID")
	if resp.Blog.AuthorId != userID && !followerPermission(r, userID, resp.Blog.AuthorId, "can-read") {
		http.Error(w, `{"error":"follow this author to read this blog"}`, http.StatusForbidden)
		return
	}

	w.Header().Set("Content-Type", "application/json")
	json.NewEncoder(w).Encode(resp.Blog)
}

func AddCommentWithFollowerCheck(w http.ResponseWriter, r *http.Request) {
	if !canAccessBlogAuthor(w, r, "can-comment", "follow this author to comment on this blog") {
		return
	}

	ProxyBlog(w, r)
}

func ProxyBlogWithReadCheck(w http.ResponseWriter, r *http.Request) {
	if !canAccessBlogAuthor(w, r, "can-read", "follow this author to read this blog") {
		return
	}

	ProxyBlog(w, r)
}

func canAccessBlogAuthor(w http.ResponseWriter, r *http.Request, permission string, message string) bool {
	vars := mux.Vars(r)
	blogID := vars["blogId"]

	resp, err := grpcclient.GetBlogByIDRPC(blogID)
	if err != nil {
		http.Error(w, err.Error(), http.StatusInternalServerError)
		return false
	}

	userID := r.Header.Get("X-User-ID")
	if resp.Blog.AuthorId != userID && !followerPermission(r, userID, resp.Blog.AuthorId, permission) {
		http.Error(w, fmt.Sprintf(`{"error":"%s"}`, message), http.StatusForbidden)
		return false
	}

	return true
}

func followerPermission(r *http.Request, userID, authorID, permission string) bool {
	if userID == "" || authorID == "" {
		return false
	}

	endpoint := fmt.Sprintf("%s/followers/%s/%s/%s", config.ServiceURLs["follower"], userID, permission, authorID)
	req, err := http.NewRequestWithContext(r.Context(), http.MethodGet, endpoint, nil)
	if err != nil {
		return false
	}

	req.Header.Set("Authorization", r.Header.Get("Authorization"))
	client := &http.Client{Timeout: 3 * time.Second}
	resp, err := client.Do(req)
	if err != nil {
		return false
	}
	defer resp.Body.Close()

	if resp.StatusCode != http.StatusOK {
		io.Copy(io.Discard, resp.Body)
		return false
	}

	var permissionResponse followerPermissionResponse
	if err := json.NewDecoder(resp.Body).Decode(&permissionResponse); err != nil {
		return false
	}

	switch permission {
	case "can-read":
		return permissionResponse.CanRead
	case "can-comment":
		return permissionResponse.CanComment
	default:
		return false
	}
}
