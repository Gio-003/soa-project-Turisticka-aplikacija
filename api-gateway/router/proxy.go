package router

import (
	"api-gateway/config"
	"io"
	"log"
	"net/http"
	"net/url"
)

// ProxyBlog routes requests to the blog service
func ProxyBlog(w http.ResponseWriter, r *http.Request) {
	forwardRequest(w, r, config.ServiceURLs["blog"])
}

// ProxyAuth routes requests to the stakeholders service
func ProxyAuth(w http.ResponseWriter, r *http.Request) {
	forwardRequest(w, r, config.ServiceURLs["stakeholders"])
}

// forwardRequest forwards the request to the target service
func forwardRequest(w http.ResponseWriter, r *http.Request, targetURL string) {
	// Build the target URL
	target, err := url.Parse(targetURL)
	if err != nil {
		log.Printf("Error parsing target URL: %v", err)
		http.Error(w, `{"error":"Invalid target URL"}`, http.StatusBadGateway)
		return
	}

	// Get path from original request
	rawPath := r.URL.Path

	// If path contains /api prefix, remove it for backend services
	if len(rawPath) > 4 && rawPath[:4] == "/api" {
		rawPath = rawPath[4:]
	}

	target.Path = rawPath
	target.RawQuery = r.URL.RawQuery

	// Create new request to backend service
	proxyReq, err := http.NewRequest(r.Method, target.String(), r.Body)
	if err != nil {
		log.Printf("Error creating proxy request: %v", err)
		http.Error(w, `{"error":"Error creating proxy request"}`, http.StatusBadGateway)
		return
	}

	// Copy headers (except host)
	for key, values := range r.Header {
		for _, value := range values {
			if key != "Host" && key != "X-Forwarded-For" {
				proxyReq.Header.Add(key, value)
			}
		}
	}

	// Add X-Forwarded headers
	proxyReq.Header.Add("X-Forwarded-For", r.RemoteAddr)
	proxyReq.Header.Add("X-Forwarded-Proto", "http")
	proxyReq.Header.Add("X-Forwarded-Host", r.Host)

	log.Printf("Forwarding %s %s to %s", r.Method, rawPath, target.String())

	// Execute the request
	client := &http.Client{}
	resp, err := client.Do(proxyReq)
	if err != nil {
		log.Printf("Error forwarding request: %v", err)
		http.Error(w, `{"error":"Service unavailable"}`, http.StatusServiceUnavailable)
		return
	}
	defer resp.Body.Close()

	// Copy response headers
	for key, values := range resp.Header {
		for _, value := range values {
			w.Header().Add(key, value)
		}
	}

	// Copy status code and body
	w.WriteHeader(resp.StatusCode)
	_, err = io.Copy(w, resp.Body)
	if err != nil {
		log.Printf("Error copying response body: %v", err)
	}
}
