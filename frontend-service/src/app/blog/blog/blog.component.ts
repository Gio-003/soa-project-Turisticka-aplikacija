import { Component, OnInit } from '@angular/core';
import { BlogService } from '../blog.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-blog',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './blog.component.html',
  styleUrls: ['./blog.component.css'],
})
export class BlogComponent implements OnInit {
  blogs: any[] = [];
  newBlogTitle: string = '';
  newBlogDescription: string = '';

  constructor(private blogService: BlogService) {}

  ngOnInit(): void {
    this.loadBlogs();
  }

  loadBlogs(): void {
    this.blogService.getAllBlogs().subscribe({
      next: (data) => {
        this.blogs = data;
      },
      error: (err) => console.error('Error loading blogs:', err),
    });
  }

  createBlog(): void {
    if (this.newBlogTitle && this.newBlogDescription) {
      const blogData = {
        title: this.newBlogTitle,
        description: this.newBlogDescription, // Proveri da li se u Go-u polje zove 'description' ili 'content'
      };
      this.blogService.createBlog(blogData).subscribe({
        next: () => {
          this.newBlogTitle = '';
          this.newBlogDescription = '';
          this.loadBlogs();
        },
        error: (err) => console.error('Error creating blog:', err),
      });
    }
  }

  likeBlog(blogId: string): void {
    this.blogService.likeBlog(blogId).subscribe({
      next: () => this.loadBlogs(),
      error: (err) => console.error('Error liking blog:', err),
    });
  }

  unlikeBlog(blogId: string): void {
    this.blogService.unlikeBlog(blogId).subscribe({
      next: () => this.loadBlogs(),
      error: (err) => console.error('Error unliking blog:', err),
    });
  }

  // Sada funkcija prima tekst direktno iz HTML inputa
  addComment(blogId: string, commentText: string): void {
    if (commentText.trim()) {
      const commentData = { content: commentText };
      this.blogService.addComment(blogId, commentData).subscribe({
        next: () => {
          this.loadBlogs();
        },
        error: (err) => console.error('Error adding comment:', err),
      });
    }
  }
}