import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../services/api.service';

@Injectable({
  providedIn: 'root'
})
export class BlogService {
  private apiUrl = 'http://localhost:8000/api/blogs';
  private authorsUrl = 'http://localhost:8000/api/blog-authors';

  constructor(private apiService: ApiService) { }

getAllBlogs(): Observable<any[]> {
  return this.apiService.get(this.apiUrl);
}

getBlogAuthors(): Observable<{ id: string; blogCount: number }[]> {
  return this.apiService.get(this.authorsUrl);
}

getBlogById(id: string): Observable<any> {
  return this.apiService.get(`${this.apiUrl}/${id}`);
}

  createBlog(blogData: any): Observable<any> {
    return this.apiService.post(this.apiUrl, blogData);
  }

  likeBlog(blogId: string): Observable<any> {
    return this.apiService.post(`${this.apiUrl}/${blogId}/likes`, {});
  }

  unlikeBlog(blogId: string): Observable<any> {
    return this.apiService.delete(`${this.apiUrl}/${blogId}/likes`);
  }

  addComment(blogId: string, commentData: any): Observable<any> {
    return this.apiService.post(`${this.apiUrl}/${blogId}/comments`, commentData);
  }
}
