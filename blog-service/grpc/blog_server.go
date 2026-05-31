package grpc

import (
	pb "blog-service/proto"
	"blog-service/service"
	"context"

	"google.golang.org/protobuf/types/known/timestamppb"
)

type BlogGrpcServer struct {
	pb.UnimplementedBlogServiceServer
	Service *service.BlogService
}

func (s *BlogGrpcServer) GetAllBlogs(ctx context.Context, req *pb.EmptyRequest) (*pb.BlogListResponse, error) {

	blogs, err := s.Service.GetAllBlogs()
	if err != nil {
		return nil, err
	}

	var result []*pb.BlogMessage

	for _, blog := range blogs {

		var comments []*pb.CommentMessage

		for _, c := range blog.Comments {
			comments = append(comments, &pb.CommentMessage{
				Id:        c.ID.Hex(),
				AuthorId:  c.AuthorID,
				Content:   c.Content,
				CreatedAt: timestamppb.New(c.CreatedAt),
				UpdatedAt: timestamppb.New(c.UpdatedAt),
			})
		}

		result = append(result, &pb.BlogMessage{
			Id:          blog.ID.Hex(),
			Title:       blog.Title,
			Description: blog.Description,
			CreatedAt:   timestamppb.New(blog.CreatedAt),
			ImageUrl:    blog.ImageURL,
			AuthorId:    blog.AuthorID,
			Likes:       blog.Likes,
			Comments:    comments,
		})
	}

	return &pb.BlogListResponse{
		Blogs: result,
	}, nil
}
func (s *BlogGrpcServer) GetBlogById(ctx context.Context, req *pb.BlogIdRequest) (*pb.BlogResponse, error) {

	blog, err := s.Service.GetBlogByID(req.Id)
	if err != nil {
		return nil, err
	}

	var comments []*pb.CommentMessage

	for _, c := range blog.Comments {
		comments = append(comments, &pb.CommentMessage{
			Id:        c.ID.Hex(),
			AuthorId:  c.AuthorID,
			Content:   c.Content,
			CreatedAt: timestamppb.New(c.CreatedAt),
			UpdatedAt: timestamppb.New(c.UpdatedAt),
		})
	}

	return &pb.BlogResponse{
		Blog: &pb.BlogMessage{
			Id:          blog.ID.Hex(),
			Title:       blog.Title,
			Description: blog.Description,
			CreatedAt:   timestamppb.New(blog.CreatedAt),
			ImageUrl:    blog.ImageURL,
			AuthorId:    blog.AuthorID,
			Likes:       blog.Likes,
			Comments:    comments,
		},
	}, nil
}
