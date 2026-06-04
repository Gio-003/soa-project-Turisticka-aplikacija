package grpc

import (
	pb "api-gateway/proto" // Proveri da li je ovo ispravna putanja do tvog generisanog proto koda
	"context"
	"log"
	"time"

	"google.golang.org/grpc"
	"google.golang.org/grpc/credentials/insecure"
)

// Globalne varijable koje će biti dostupne tvojim handlerima
var (
	BlogClient pb.BlogServiceClient
	conn       *grpc.ClientConn
)

// InitBlogClient se poziva samo jednom, obično u main.go Gateway-a pri pokretanju.
// Ova funkcija uspostavlja stalnu vezu sa Blog servisom.
func InitBlogClient() {
	var err error
	
	// Koristimo insecure jer verovatno nemaš podešene SSL sertifikate za lokalni razvoj.
	// "blog-service:50051" pretpostavlja da su servisi u istoj Docker mreži.
	// Ako testiraš van Dockera, promeni u "localhost:50051".
	conn, err = grpc.Dial(
		//"blog-service:50051",
		"localhost:50051", 
		grpc.WithTransportCredentials(insecure.NewCredentials()),
	)
	
	if err != nil {
		log.Fatalf("Greška pri povezivanju na Blog servis: %v", err)
	}
	
	// Inicijalizujemo klijenta kojeg će svi pozivi koristiti
	BlogClient = pb.NewBlogServiceClient(conn)
	log.Println("✅ gRPC Blog klijent uspešno povezan na :50051")
}

// CloseBlogClient zatvara konekciju kada se Gateway gasi.
func CloseBlogClient() {
	if conn != nil {
		conn.Close()
	}
}

// GetAllBlogsRPC poziva gRPC metodu servisa za listu svih blogova
func GetAllBlogsRPC() (*pb.BlogListResponse, error) {
	ctx, cancel := context.WithTimeout(context.Background(), 5*time.Second)
	defer cancel()

	return BlogClient.GetAllBlogs(ctx, &pb.EmptyRequest{})
}

// GetBlogByIDRPC poziva gRPC metodu servisa za jedan blog na osnovu ID-a
func GetBlogByIDRPC(id string) (*pb.BlogResponse, error) {
	ctx, cancel := context.WithTimeout(context.Background(), 5*time.Second)
	defer cancel()

	return BlogClient.GetBlogById(ctx, &pb.BlogIdRequest{
		Id: id,
	})
}