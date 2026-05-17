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
    return this.http.post<KeyPoint>(`${this.config.apiUrl}/tours/${tourId}/keypoints`, keyPoint);
  }

  getKeyPointsForTour(tourId: string): Observable<KeyPoint[]> {
    return this.http.get<KeyPoint[]>(`${this.config.apiUrl}/tours/${tourId}/keypoints`);
  }
}