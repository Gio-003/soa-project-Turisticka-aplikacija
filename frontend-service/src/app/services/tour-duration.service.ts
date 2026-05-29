// src/app/services/tour-duration.service.ts
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { ConfigService } from './config.service';
import { TourDuration } from '../models/tour-duration.model';

@Injectable({
  providedIn: 'root'
})
export class TourDurationService {

  constructor(
    private apiService: ApiService,
    private config: ConfigService
  ) {}

  addDuration(
    tourId: string,
    duration: Omit<TourDuration, 'id' | 'tourId'>
  ): Observable<TourDuration> {
    return this.apiService.post(
      `${this.config.tours_url}/${tourId}/durations`,
      duration
    );
  }

  getDurationsForTour(tourId: string): Observable<TourDuration[]> {
    return this.apiService.get(
      `${this.config.tours_url}/${tourId}/durations`
    );
  }

  updateDuration(
    tourId: string,
    durationId: string,
    duration: Omit<TourDuration, 'id' | 'tourId'>
  ): Observable<TourDuration> {
    return this.apiService.put(
      `${this.config.tours_url}/${tourId}/durations/${durationId}`,
      duration
    );
  }

  deleteDuration(tourId: string, durationId: string): Observable<void> {
    return this.apiService.delete(
      `${this.config.tours_url}/${tourId}/durations/${durationId}`
    );
  }
}