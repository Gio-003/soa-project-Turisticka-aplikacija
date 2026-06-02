import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { ConfigService } from './config.service';

export interface CreateTourRequest {
  name: string;
  description: string;
  difficulty: string;
  tags: string[];
  price?: number;
}

@Injectable({
  providedIn: 'root'
})
export class TourService {

  constructor(
    private apiService: ApiService,
    private config: ConfigService
  ) { }

  createTour(request: CreateTourRequest) {
    return this.apiService.post(
      this.config.tours_url,
      request
    );
  }

  getMyTours(authorId: number) {
    return this.apiService.get(
      `${this.config.tours_url}/my/${authorId}`
    );
  }

  getAllTours() {
    return this.apiService.get(
      this.config.tours_url + '/all'
    );
  }

  getTourById(id: string) {
    return this.apiService.get(
      `${this.config.tours_url}/${id}`
    );
  }

  publishTour(id: string) {
    return this.apiService.post(
      `${this.config.tours_url}/${id}/publish`,
      {}
    );
  }

  archiveTour(id: string) {
    return this.apiService.post(
      `${this.config.tours_url}/${id}/archive`,
      {}
    );
  }
  updateTourLength(tourId: string, lengthInKm: number) {
    return this.apiService.put(
      `${this.config.tours_url}/${tourId}/length`,
      {
        lengthInKm: lengthInKm
      }
    );
  }

  private get executionBaseUrl(): string {
    return this.config.apiUrl + '/tour-executions';
  }

  startExecution(tourId: string, touristId: number, latitude: number, longitude: number) {
    return this.apiService.post(this.executionBaseUrl + '/start', {
      tourId,
      touristId,
      latitude,
      longitude
    });
  }

  checkPosition(executionId: string, latitude: number, longitude: number) {
    return this.apiService.post(this.executionBaseUrl + '/check-position', {
      tourExecutionId: executionId,
      latitude,
      longitude
    });
  }

  completeExecution(executionId: string) {
    return this.apiService.post(`${this.executionBaseUrl}/${executionId}/complete`, {});
  }

  abandonExecution(executionId: string) {
    return this.apiService.post(`${this.executionBaseUrl}/${executionId}/abandon`, {});
  }

  getExecution(executionId: string) {
    return this.apiService.get(`${this.executionBaseUrl}/${executionId}`);
  }
}
