// src/app/services/key-point.service.ts
import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { Observable } from 'rxjs';
import { KeyPoint } from '../models/key-point.model';
import { ConfigService } from './config.service';

@Injectable({
  providedIn: 'root'
})

export class KeyPointService {

  constructor(
    private apiService: ApiService,
    private config: ConfigService
) { }

  addKeyPoint(tourId: string, keyPoint: Omit<KeyPoint, 'id' | 'tourId'>): Observable<KeyPoint> {
    return this.apiService.post(`${this.config.tours_url}/${tourId}/keypoints`, keyPoint);
  }

  getKeyPointsForTour(tourId: string): Observable<KeyPoint[]> {
    return this.apiService.get(`${this.config.tours_url}/${tourId}/keypoints`);
  }

  updateKeyPoint(tourId: string, keyPointId: string, keyPoint: Omit<KeyPoint, 'id' | 'tourId'>): Observable<KeyPoint> {
    return this.apiService.put(`${this.config.tours_url}/${tourId}/keypoints/${keyPointId}`, keyPoint);
  }

  deleteKeyPoint(tourId: string, keyPointId: string): Observable<void> {
    return this.apiService.delete(`${this.config.tours_url}/${tourId}/keypoints/${keyPointId}`);
  }
}