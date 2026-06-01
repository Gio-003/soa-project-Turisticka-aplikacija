import { Component, OnInit } from '@angular/core';
import { BlogService } from '../blog.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { FollowerService } from '../../services/follower.service';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-blog',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './blog.component.html',
  styleUrls: ['./blog.component.css'],
})
export class BlogComponent implements OnInit {
  blogs: any[] = [];
  visibleBlogs: any[] = [];
  lockedBlogs: any[] = [];
  recommendations: { id: string }[] = [];
  followingIds = new Set<string>();
  newBlogTitle = '';
  newBlogDescription = '';
  message = '';

  constructor(
    private blogService: BlogService,
    private followerService: FollowerService,
    public userService: UserService
  ) {}

  ngOnInit(): void {
    this.loadFollowerState();
  }

  currentUserId(): string {
    return String(this.userService.currentUser?.id || '');
  }

  loadFollowerState(): void {
    const userId = this.currentUserId();
    if (!userId) {
      this.loadBlogs();
      return;
    }

    this.followerService.getFollowing(userId).subscribe({
      next: response => {
        this.followingIds = new Set((response.users || []).map(user => String(user.id)));
        this.loadBlogs();
      },
      error: () => this.loadBlogs()
    });

    this.followerService.getRecommendations(userId).subscribe({
      next: response => {
        this.recommendations = response.users || [];
      },
      error: () => {
        this.recommendations = [];
      }
    });
  }

  loadBlogs(): void {
    this.blogService.getAllBlogs().subscribe({
      next: data => {
        this.blogs = data || [];
        this.applyBlogVisibility();
      },
      error: err => console.error('Error loading blogs:', err),
    });
  }

  applyBlogVisibility(): void {
    const userId = this.currentUserId();
    this.visibleBlogs = [];
    this.lockedBlogs = [];

    this.blogs.forEach(blog => {
      const authorId = String(blog.authorId);
      if (authorId === userId || this.followingIds.has(authorId)) {
        this.visibleBlogs.push(blog);
      } else {
        this.lockedBlogs.push(blog);
      }
    });
  }

  createBlog(): void {
    if (this.newBlogTitle && this.newBlogDescription) {
      const blogData = {
        title: this.newBlogTitle,
        description: this.newBlogDescription,
      };
      this.blogService.createBlog(blogData).subscribe({
        next: () => {
          this.newBlogTitle = '';
          this.newBlogDescription = '';
          this.loadFollowerState();
        },
        error: err => console.error('Error creating blog:', err),
      });
    }
  }

  follow(authorId: string): void {
    const userId = this.currentUserId();
    this.followerService.follow(userId, authorId).subscribe({
      next: () => {
        this.message = 'Profile followed.';
        this.loadFollowerState();
      }
    });
  }

  unfollow(authorId: string): void {
    const userId = this.currentUserId();
    this.followerService.unfollow(userId, authorId).subscribe({
      next: () => {
        this.message = 'Profile unfollowed.';
        this.loadFollowerState();
      }
    });
  }

  isOwnBlog(blog: any): boolean {
    return String(blog.authorId) === this.currentUserId();
  }

  canComment(blog: any): boolean {
    return this.isOwnBlog(blog) || this.followingIds.has(String(blog.authorId));
  }

  likeBlog(blogId: string): void {
    this.blogService.likeBlog(blogId).subscribe({
      next: () => this.loadBlogs(),
      error: err => console.error('Error liking blog:', err),
    });
  }

  unlikeBlog(blogId: string): void {
    this.blogService.unlikeBlog(blogId).subscribe({
      next: () => this.loadBlogs(),
      error: err => console.error('Error unliking blog:', err),
    });
  }

  addComment(blog: any, commentText: string): void {
    if (!this.canComment(blog)) {
      this.message = 'Follow this author to comment.';
      return;
    }

    if (commentText.trim()) {
      const commentData = { content: commentText };
      this.blogService.addComment(blog.id, commentData).subscribe({
        next: () => {
          this.loadBlogs();
        },
        error: err => console.error('Error adding comment:', err),
      });
    }
  }
}
