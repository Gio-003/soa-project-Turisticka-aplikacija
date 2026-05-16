package middleware

import (
	"net/http"
)

func AdminMiddleware(next http.Handler) http.Handler {
	return http.HandlerFunc(func(w http.ResponseWriter, r *http.Request) {
		userRole := r.Header.Get("X-User-Role")
		
		// Role is stored as integer string (0 = ADMIN, 1 = GUIDE, 2 = TOURIST)
		if userRole != "0" && userRole != "ADMIN" {
			http.Error(w, `{"error":"admin access required"}`, http.StatusForbidden)
			return
		}

		next.ServeHTTP(w, r)
	})
}
