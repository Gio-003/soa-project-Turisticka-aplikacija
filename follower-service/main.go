package main

import (
	"context"
	"encoding/json"
	"errors"
	"log"
	"net/http"
	"os"
	"strings"
	"time"

	"github.com/gorilla/mux"
	"github.com/neo4j/neo4j-go-driver/v5/neo4j"
)

type UserResponse struct {
	ID string `json:"id"`
}

type UsersResponse struct {
	Users []UserResponse `json:"users"`
}

type FollowStatusResponse struct {
	IsFollowing bool `json:"isFollowing"`
}

type CanReadResponse struct {
	CanRead bool `json:"canRead"`
}

type CanCommentResponse struct {
	CanComment bool `json:"canComment"`
}

type MessageResponse struct {
	Message string `json:"message"`
}

type ErrorResponse struct {
	Error string `json:"error"`
}

type FollowerRepository struct {
	driver neo4j.DriverWithContext
}

func NewFollowerRepository(driver neo4j.DriverWithContext) *FollowerRepository {
	return &FollowerRepository{driver: driver}
}

func (r *FollowerRepository) Follow(ctx context.Context, followerID, followedID string) error {
	query := `
		MERGE (follower:User {id: $followerId})
		MERGE (followed:User {id: $followedId})
		MERGE (follower)-[:FOLLOWS]->(followed)
	`
	_, err := neo4j.ExecuteQuery(ctx, r.driver, query, map[string]any{
		"followerId": followerID,
		"followedId": followedID,
	}, neo4j.EagerResultTransformer, neo4j.ExecuteQueryWithDatabase(""))
	return err
}

func (r *FollowerRepository) Unfollow(ctx context.Context, followerID, followedID string) error {
	query := `
		MATCH (:User {id: $followerId})-[rel:FOLLOWS]->(:User {id: $followedId})
		DELETE rel
	`
	_, err := neo4j.ExecuteQuery(ctx, r.driver, query, map[string]any{
		"followerId": followerID,
		"followedId": followedID,
	}, neo4j.EagerResultTransformer, neo4j.ExecuteQueryWithDatabase(""))
	return err
}

func (r *FollowerRepository) Following(ctx context.Context, followerID string) ([]UserResponse, error) {
	query := `
		MATCH (:User {id: $followerId})-[:FOLLOWS]->(followed:User)
		RETURN DISTINCT followed.id AS id
		ORDER BY id
	`
	return r.readUsers(ctx, query, map[string]any{"followerId": followerID})
}

func (r *FollowerRepository) Followers(ctx context.Context, userID string) ([]UserResponse, error) {
	query := `
		MATCH (follower:User)-[:FOLLOWS]->(:User {id: $userId})
		RETURN DISTINCT follower.id AS id
		ORDER BY id
	`
	return r.readUsers(ctx, query, map[string]any{"userId": userID})
}

func (r *FollowerRepository) IsFollowing(ctx context.Context, followerID, followedID string) (bool, error) {
	query := `
		RETURN EXISTS {
			MATCH (:User {id: $followerId})-[:FOLLOWS]->(:User {id: $followedId})
		} AS isFollowing
	`
	result, err := neo4j.ExecuteQuery(ctx, r.driver, query, map[string]any{
		"followerId": followerID,
		"followedId": followedID,
	}, neo4j.EagerResultTransformer, neo4j.ExecuteQueryWithDatabase(""))
	if err != nil {
		return false, err
	}
	if len(result.Records) == 0 {
		return false, nil
	}
	value, ok := result.Records[0].Get("isFollowing")
	if !ok {
		return false, nil
	}
	isFollowing, _ := value.(bool)
	return isFollowing, nil
}

func (r *FollowerRepository) Recommendations(ctx context.Context, userID string) ([]UserResponse, error) {
	query := `
		MATCH (current:User {id: $userId})-[:FOLLOWS]->(:User)-[:FOLLOWS]->(recommended:User)
		WHERE recommended.id <> $userId
		  AND NOT (current)-[:FOLLOWS]->(recommended)
		RETURN DISTINCT recommended.id AS id
		ORDER BY id
	`
	return r.readUsers(ctx, query, map[string]any{"userId": userID})
}

func (r *FollowerRepository) readUsers(ctx context.Context, query string, params map[string]any) ([]UserResponse, error) {
	result, err := neo4j.ExecuteQuery(ctx, r.driver, query, params, neo4j.EagerResultTransformer, neo4j.ExecuteQueryWithDatabase(""))
	if err != nil {
		return nil, err
	}

	users := make([]UserResponse, 0, len(result.Records))
	for _, record := range result.Records {
		value, ok := record.Get("id")
		if !ok {
			continue
		}
		id, ok := value.(string)
		if !ok {
			continue
		}
		users = append(users, UserResponse{ID: id})
	}
	return users, nil
}

func (r *FollowerRepository) EnsureSchema(ctx context.Context) error {
	query := `
		CREATE CONSTRAINT user_id_unique IF NOT EXISTS
		FOR (u:User) REQUIRE u.id IS UNIQUE
	`
	_, err := neo4j.ExecuteQuery(ctx, r.driver, query, nil, neo4j.EagerResultTransformer, neo4j.ExecuteQueryWithDatabase(""))
	return err
}

type FollowerHandler struct {
	repository *FollowerRepository
}

func NewFollowerHandler(repository *FollowerRepository) *FollowerHandler {
	return &FollowerHandler{repository: repository}
}

func (h *FollowerHandler) Follow(w http.ResponseWriter, r *http.Request) {
	followerID, followedID := mux.Vars(r)["followerId"], mux.Vars(r)["followedId"]
	if err := validateFollowRequest(followerID, followedID); err != nil {
		writeError(w, http.StatusBadRequest, err.Error())
		return
	}

	if err := h.repository.Follow(r.Context(), followerID, followedID); err != nil {
		log.Printf("follow failed: %v", err)
		writeError(w, http.StatusInternalServerError, "failed to follow user")
		return
	}
	writeJSON(w, http.StatusOK, MessageResponse{Message: "user followed"})
}

func (h *FollowerHandler) Unfollow(w http.ResponseWriter, r *http.Request) {
	followerID, followedID := mux.Vars(r)["followerId"], mux.Vars(r)["followedId"]
	if err := validateFollowRequest(followerID, followedID); err != nil {
		writeError(w, http.StatusBadRequest, err.Error())
		return
	}

	if err := h.repository.Unfollow(r.Context(), followerID, followedID); err != nil {
		log.Printf("unfollow failed: %v", err)
		writeError(w, http.StatusInternalServerError, "failed to unfollow user")
		return
	}
	writeJSON(w, http.StatusOK, MessageResponse{Message: "user unfollowed"})
}

func (h *FollowerHandler) Following(w http.ResponseWriter, r *http.Request) {
	users, err := h.repository.Following(r.Context(), mux.Vars(r)["followerId"])
	if err != nil {
		log.Printf("get following failed: %v", err)
		writeError(w, http.StatusInternalServerError, "failed to get following")
		return
	}
	writeJSON(w, http.StatusOK, UsersResponse{Users: users})
}

func (h *FollowerHandler) Followers(w http.ResponseWriter, r *http.Request) {
	users, err := h.repository.Followers(r.Context(), mux.Vars(r)["followerId"])
	if err != nil {
		log.Printf("get followers failed: %v", err)
		writeError(w, http.StatusInternalServerError, "failed to get followers")
		return
	}
	writeJSON(w, http.StatusOK, UsersResponse{Users: users})
}

func (h *FollowerHandler) IsFollowing(w http.ResponseWriter, r *http.Request) {
	vars := mux.Vars(r)
	isFollowing, err := h.repository.IsFollowing(r.Context(), vars["followerId"], vars["followedId"])
	if err != nil {
		log.Printf("is-following failed: %v", err)
		writeError(w, http.StatusInternalServerError, "failed to check follow status")
		return
	}
	writeJSON(w, http.StatusOK, FollowStatusResponse{IsFollowing: isFollowing})
}

func (h *FollowerHandler) Recommendations(w http.ResponseWriter, r *http.Request) {
	users, err := h.repository.Recommendations(r.Context(), mux.Vars(r)["userId"])
	if err != nil {
		log.Printf("recommendations failed: %v", err)
		writeError(w, http.StatusInternalServerError, "failed to get recommendations")
		return
	}
	writeJSON(w, http.StatusOK, UsersResponse{Users: users})
}

func (h *FollowerHandler) CanRead(w http.ResponseWriter, r *http.Request) {
	vars := mux.Vars(r)
	canRead, err := h.repository.IsFollowing(r.Context(), vars["userId"], vars["authorId"])
	if err != nil {
		log.Printf("can-read failed: %v", err)
		writeError(w, http.StatusInternalServerError, "failed to check read permission")
		return
	}
	writeJSON(w, http.StatusOK, CanReadResponse{CanRead: canRead})
}

func (h *FollowerHandler) CanComment(w http.ResponseWriter, r *http.Request) {
	vars := mux.Vars(r)
	canComment, err := h.repository.IsFollowing(r.Context(), vars["userId"], vars["authorId"])
	if err != nil {
		log.Printf("can-comment failed: %v", err)
		writeError(w, http.StatusInternalServerError, "failed to check comment permission")
		return
	}
	writeJSON(w, http.StatusOK, CanCommentResponse{CanComment: canComment})
}

func validateFollowRequest(followerID, followedID string) error {
	if strings.TrimSpace(followerID) == "" || strings.TrimSpace(followedID) == "" {
		return errors.New("user ids are required")
	}
	if followerID == followedID {
		return errors.New("user cannot follow himself")
	}
	return nil
}

func writeJSON(w http.ResponseWriter, status int, body any) {
	w.Header().Set("Content-Type", "application/json")
	w.WriteHeader(status)
	if err := json.NewEncoder(w).Encode(body); err != nil {
		log.Printf("failed to encode response: %v", err)
	}
}

func writeError(w http.ResponseWriter, status int, message string) {
	writeJSON(w, status, ErrorResponse{Error: message})
}

func main() {
	neo4jURI := getEnv("NEO4J_URI", "bolt://localhost:7687")
	neo4jUsername := getEnv("NEO4J_USERNAME", "neo4j")
	neo4jPassword := getEnv("NEO4J_PASSWORD", "password")
	port := getEnv("PORT", "8083")

	driver, err := neo4j.NewDriverWithContext(neo4jURI, neo4j.BasicAuth(neo4jUsername, neo4jPassword, ""))
	if err != nil {
		log.Fatalf("failed to create Neo4j driver: %v", err)
	}
	defer func() {
		if err := driver.Close(context.Background()); err != nil {
			log.Printf("failed to close Neo4j driver: %v", err)
		}
	}()

	ctx, cancel := context.WithTimeout(context.Background(), 30*time.Second)
	defer cancel()
	if err := driver.VerifyConnectivity(ctx); err != nil {
		log.Fatalf("failed to connect to Neo4j: %v", err)
	}

	repository := NewFollowerRepository(driver)
	if err := repository.EnsureSchema(ctx); err != nil {
		log.Fatalf("failed to ensure Neo4j schema: %v", err)
	}

	handler := NewFollowerHandler(repository)
	router := mux.NewRouter()
	router.HandleFunc("/health", func(w http.ResponseWriter, r *http.Request) {
		writeJSON(w, http.StatusOK, map[string]string{"status": "healthy", "service": "follower-service"})
	}).Methods(http.MethodGet)
	router.HandleFunc("/followers/{followerId}/follow/{followedId}", handler.Follow).Methods(http.MethodPost)
	router.HandleFunc("/followers/{followerId}/follow/{followedId}", handler.Unfollow).Methods(http.MethodDelete)
	router.HandleFunc("/followers/{followerId}/following", handler.Following).Methods(http.MethodGet)
	router.HandleFunc("/followers/{followerId}/followers", handler.Followers).Methods(http.MethodGet)
	router.HandleFunc("/followers/{followerId}/is-following/{followedId}", handler.IsFollowing).Methods(http.MethodGet)
	router.HandleFunc("/followers/{userId}/recommendations", handler.Recommendations).Methods(http.MethodGet)
	// Permission decision endpoints for blog/comment access. Enforcement in blog-service is intentionally out of scope here.
	router.HandleFunc("/followers/{userId}/can-read/{authorId}", handler.CanRead).Methods(http.MethodGet)
	router.HandleFunc("/followers/{userId}/can-comment/{authorId}", handler.CanComment).Methods(http.MethodGet)

	log.Printf("Follower service starting on port %s", port)
	if err := http.ListenAndServe(":"+port, router); err != nil {
		log.Fatalf("failed to start follower service: %v", err)
	}
}

func getEnv(key, fallback string) string {
	value := os.Getenv(key)
	if value == "" {
		return fallback
	}
	return value
}
