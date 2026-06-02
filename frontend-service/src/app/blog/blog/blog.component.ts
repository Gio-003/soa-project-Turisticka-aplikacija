import { Component, OnInit } from '@angular/core';
import { BlogService } from '../blog.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { FollowerService } from '../../services/follower.service';
import { UserService } from '../../services/user.service';
import { AuthService } from '../../services/auth.service';

type LockedAuthor = {
  id: string;
  blogCount: number;
};

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
  lockedAuthors: LockedAuthor[] = [];
  recommendations: { id: string }[] = [];
  followingIds = new Set<string>();
  newBlogTitle = '';
  newBlogDescription = '';
  message = '';
  isLoading = true;

  constructor(
    private blogService: BlogService,
    private followerService: FollowerService,
    public userService: UserService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    if (!this.userService.currentUser && this.authService.tokenIsPresent()) {
      this.userService.getMyInfo().subscribe({
        next: () => this.loadFollowerState(),
        error: () => {
          this.authService.clearSession();
          this.isLoading = false;
          this.message = 'Login again to view blogs.';
        }
      });
      return;
    }

    this.loadFollowerState();
  }

  currentUserId(): string {
    return String(this.userService.currentUser?.id || this.userService.currentUser?.Id || '');
  }

  loadFollowerState(): void {
    const userId = this.currentUserId();
    if (!userId) {
      this.isLoading = false;
      return;
    }

    this.isLoading = true;
    this.followerService.getFollowing(userId).subscribe({
      next: response => {
        this.followingIds = new Set((response.users || []).map(user => String(user.id)));
        this.loadBlogs();
        this.loadLockedAuthors();
      },
      error: () => {
        this.loadBlogs();
        this.loadLockedAuthors();
      }
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
        this.isLoading = false;
      },
      error: err => {
        this.isLoading = false;
        this.message = 'Could not load blogs.';
        console.error('Error loading blogs:', err);
      },
    });
  }

  loadLockedAuthors(): void {
    this.blogService.getBlogAuthors().subscribe({
      next: authors => {
        this.lockedAuthors = (authors || []).filter(author =>
          String(author.id) !== this.currentUserId() && !this.followingIds.has(String(author.id))
        );
      },
      error: err => {
        this.lockedAuthors = [];
        console.error('Error loading blog authors:', err);
      },
    });
  }

  applyBlogVisibility(): void {
    const userId = this.currentUserId();
    this.visibleBlogs = [];

    this.blogs.forEach(blog => {
      const authorId = String(blog.authorId);
      if (authorId === userId || this.followingIds.has(authorId)) {
        this.visibleBlogs.push(blog);
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
    if (!userId) {
      this.message = 'Login again before following authors.';
      return;
    }
    if (String(authorId) === userId) {
      this.message = 'You cannot follow yourself.';
      return;
    }

    this.followerService.follow(userId, authorId).subscribe({
      next: () => {
        this.message = 'Profile followed.';
        this.loadFollowerState();
        this.loadLockedAuthors();
      },
      error: err => {
        this.message = 'Could not follow this author.';
        console.error('Error following author:', err);
      },
    });
  }

  unfollow(authorId: string): void {
    const userId = this.currentUserId();
    if (!userId) {
      this.message = 'Login again before unfollowing authors.';
      return;
    }
    if (String(authorId) === userId) {
      this.message = 'You cannot unfollow yourself.';
      return;
    }

    this.followerService.unfollow(userId, authorId).subscribe({
      next: () => {
        this.message = 'Profile unfollowed.';
        this.loadFollowerState();
        this.loadLockedAuthors();
      },
      error: err => {
        this.message = 'Could not unfollow this author.';
        console.error('Error unfollowing author:', err);
      },
    });
  }

  isOwnBlog(blog: any): boolean {
    return String(blog.authorId) === this.currentUserId();
  }

  canComment(blog: any): boolean {
    return this.isOwnBlog(blog) || this.followingIds.has(String(blog.authorId));
  }

  likesCount(blog: any): number {
    return Array.isArray(blog.likes) ? blog.likes.length : 0;
  }

  hasLiked(blog: any): boolean {
    return Array.isArray(blog.likes) && blog.likes.includes(this.currentUserId());
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
