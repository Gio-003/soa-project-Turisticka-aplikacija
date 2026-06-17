package com.example.stakeholders.grpc;

import blog.BlogServiceGrpc;
import blog.Blog;
import net.devh.boot.grpc.client.inject.GrpcClient;
import org.springframework.stereotype.Service;

@Service
public class BlogGrpcClient {

    @GrpcClient("blog-service")
    private BlogServiceGrpc.BlogServiceBlockingStub blogStub;

    public String createWelcomeBlog(String userId) {

        Blog.CreateWelcomeBlogRequest request = Blog.CreateWelcomeBlogRequest.newBuilder()
                .setUserId(userId)
                .build();

        return blogStub.createWelcomeBlog(request).getBlogId();
    }

    public void deleteBlog(String blogId) {

        Blog.DeleteBlogRequest request = Blog.DeleteBlogRequest.newBuilder()
                .setBlogId(blogId)
                .build();

        blogStub.deleteBlog(request);
    }
}