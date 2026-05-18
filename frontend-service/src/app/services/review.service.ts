import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { ConfigService } from './config.service';

export interface Review {
  id?: string;
  tourId: string;
  rating: number;
  comment: string;
  touristId: number;
  touristUsername: string;
  visitDate: string;
  createdAt?: string;
  images: string[];
}

export interface CreateReviewRequest {
  rating: number;
  comment: string;
  visitDate: string;
  images: string[];
}

@Injectable({
  providedIn: 'root'
})
export class ReviewService {

  constructor(
    private apiService: ApiService,
    private configService: ConfigService
  ) { }

  getReviews(tourId: string): Observable<Review[]> {
    const url = `${this.configService.tours_url}/${tourId}/reviews`;
    return this.apiService.get(url);
  }

  createReview(tourId: string, review: CreateReviewRequest): Observable<Review> {
    const url = `${this.configService.tours_url}/${tourId}/reviews`;
    return this.apiService.post(url, review);
  }
}
