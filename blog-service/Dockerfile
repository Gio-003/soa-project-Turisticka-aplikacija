# build stage
FROM golang:1.26.1-alpine AS builder

WORKDIR /app/blog-service

COPY blog-service/go.mod blog-service/go.sum ./
COPY common /app/common
RUN go mod download

COPY blog-service .

RUN go build -o main .

# run stage (manji image)
FROM alpine:latest

WORKDIR /app

COPY --from=builder /app/blog-service/main .

EXPOSE 8080

CMD ["./main"]
