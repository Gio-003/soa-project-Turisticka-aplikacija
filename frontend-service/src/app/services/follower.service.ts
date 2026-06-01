import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { ConfigService } from './config.service';

@Injectable({
  providedIn: 'root'
})
export class FollowerService {
  constructor(
    private apiService: ApiService,
    private config: ConfigService
  ) {}

  follow(followerId: number | string, followedId: number | string): Observable<any> {
    return this.apiService.post(`${this.config.followers_url}/${followerId}/follow/${followedId}`, {});
  }

  unfollow(followerId: number | string, followedId: number | string): Observable<any> {
    return this.apiService.delete(`${this.config.followers_url}/${followerId}/follow/${followedId}`);
  }

  getFollowing(followerId: number | string): Observable<{ users: { id: string }[] }> {
    return this.apiService.get(`${this.config.followers_url}/${followerId}/following`);
  }

  isFollowing(followerId: number | string, followedId: number | string): Observable<{ isFollowing: boolean }> {
    return this.apiService.get(`${this.config.followers_url}/${followerId}/is-following/${followedId}`);
  }

  getRecommendations(userId: number | string): Observable<{ users: { id: string }[] }> {
    return this.apiService.get(`${this.config.followers_url}/${userId}/recommendations`);
  }
}
