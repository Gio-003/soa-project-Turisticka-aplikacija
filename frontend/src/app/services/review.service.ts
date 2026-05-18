import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { ConfigService } from './config.service';

export interface CreateReviewRequest {
  rating: number;
  comment: string;
  visitDate: string;
  images: string[];
}

export interface ReviewStats {
  count: number;
  averageRating: number;
}

@Injectable({
  providedIn: 'root'
})
export class ReviewService {

  constructor(
    private apiService: ApiService,
    private config: ConfigService
  ) {}

  createReview(tourId: string, request: CreateReviewRequest) {
    return this.apiService.post(
      `${this.config.base_url}/tours/${tourId}/reviews`,
      request
    );
  }

  getReviewsByTour(tourId: string) {
    return this.apiService.get(
      `${this.config.base_url}/tours/${tourId}/reviews`
    );
  }

  getReviewStats(tourId: string) {
    return this.apiService.get(
      `${this.config.base_url}/tours/${tourId}/reviews/stats`
    );
  }

  getReviewById(reviewId: string) {
    return this.apiService.get(
      `${this.config.base_url}/reviews/${reviewId}`
    );
  }

  updateReview(reviewId: string, request: CreateReviewRequest) {
    return this.apiService.put(
      `${this.config.base_url}/reviews/${reviewId}`,
      request
    );
  }

  deleteReview(reviewId: string) {
    return this.apiService.delete(
      `${this.config.base_url}/reviews/${reviewId}`
    );
  }
}
