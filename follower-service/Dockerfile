# Build stage
FROM golang:1.26.1-alpine AS builder

WORKDIR /app

COPY go.mod go.sum* ./
RUN go mod download

COPY . .
RUN CGO_ENABLED=0 GOOS=linux go build -o follower-service .

# Run stage
FROM alpine:latest

WORKDIR /app

COPY --from=builder /app/follower-service .

EXPOSE 8083

CMD ["./follower-service"]
