// src/app/services/key-point.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { KeyPoint } from '../models/key-point.model';
import { ConfigService } from './config.service';

@Injectable({
  providedIn: 'root'
})


export class KeyPointService {

  constructor(
    private http: HttpClient,
    private config: ConfigService
) { }

  addKeyPoint(tourId: string, keyPoint: Omit<KeyPoint, 'id' | 'tourId'>): Observable<KeyPoint> {
    return this.http.post<KeyPoint>(`${this.config.tours_url}/${tourId}/keypoints`, keyPoint);
  }

  getKeyPointsForTour(tourId: string): Observable<KeyPoint[]> {
    return this.http.get<KeyPoint[]>(`${this.config.tours_url}/${tourId}/keypoints`);
  }
  updateKeyPoint(tourId: string, keyPointId: string, keyPoint: Omit<KeyPoint, 'id' | 'tourId'>): Observable<KeyPoint> {
    return this.http.put<KeyPoint>(`${this.config.tours_url}/${tourId}/keypoints/${keyPointId}`, keyPoint);
  }
  deleteKeyPoint(tourId: string, keyPointId: string): Observable<void> {
    return this.http.delete<void>(`${this.config.tours_url}/${tourId}/keypoints/${keyPointId}`);
  }
}