package grpc

import (
	pb "blog-service/proto"
	"blog-service/service"
	"blog-service/models" 
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

func (s *BlogGrpcServer) CreateWelcomeBlog(
	ctx context.Context,
	req *pb.CreateWelcomeBlogRequest,
) (*pb.CreateWelcomeBlogResponse, error) {

	blog := &models.Blog{
		Title:       "Hello everyone!",
		Description: "I am a new tour guide. Soon, you can expect tours guided by me.",
		AuthorID:    req.UserId,
	}

	err := s.Service.CreateWelcomeBlog(blog)
	if err != nil {
		return nil, err
	}

	return &pb.CreateWelcomeBlogResponse{
		BlogId: blog.ID.Hex(),
	}, nil
}

func (s *BlogGrpcServer) DeleteBlog(
	ctx context.Context,
	req *pb.DeleteBlogRequest,
) (*pb.EmptyResponse, error) {

	err := s.Service.DeleteBlog(req.BlogId)
	if err != nil {
		return nil, err
	}

	return &pb.EmptyResponse{}, nil
}
