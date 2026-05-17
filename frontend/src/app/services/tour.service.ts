import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ConfigService } from './config.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class TourService {

  constructor(
    private http: HttpClient,
    private config: ConfigService
  ) {}

  // CREATE TOUR
  createTour(data: any): Observable<any> {
    return this.http.post(
      `${this.config.apiUrl}/tours`,
      data
    );
  }

  // GET ALL TOURS OF LOGGED USER
  getMyTours(): Observable<any> {
    return this.http.get(
      `${this.config.apiUrl}/tours/my`
    );
  }

  // GET ALL TOURS (optional admin / browse)
  getAllTours(): Observable<any> {
    return this.http.get(
      `${this.config.apiUrl}/tours`
    );
  }

  // GET TOUR BY ID
  getTourById(id: number): Observable<any> {
    return this.http.get(
      `${this.config.apiUrl}/tours/${id}`
    );
  }

  // UPDATE TOUR
  updateTour(id: number, data: any): Observable<any> {
    return this.http.put(
      `${this.config.apiUrl}/tours/${id}`,
      data
    );
  }

  // DELETE TOUR
  deleteTour(id: number): Observable<any> {
    return this.http.delete(
      `${this.config.apiUrl}/tours/${id}`
    );
  }
}