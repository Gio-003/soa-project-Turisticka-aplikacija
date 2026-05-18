import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TourService } from '../../services/tour.service';
import { UserService } from '../../services/user.service';
import { SharedModule } from '../../shared/shared.module';
import { Router } from '@angular/router';

export interface TourResponse {
  id: string;
  name: string;
  description: string;
  difficulty: string;
  price: number;
  status: string;
  authorId: number;
  tags: string[];
  keyPoints?: any[];
}

@Component({
  selector: 'app-my-tours',
  standalone: true,
  imports: [CommonModule, SharedModule],
  templateUrl: './my-tours.html',
  styleUrls: ['./my-tours.css']
})
export class MyTours implements OnInit {

  tours: TourResponse[] = [];
  selectedTour: TourResponse | null = null;

  mapKeyPoints: any[] = [];

  constructor(
    private tourService: TourService,
    public userService: UserService,
    private router: Router
  ) {}

  ngOnInit(): void {
    const interval = setInterval(() => {
      const user = this.userService.currentUser;

      if (user && user.Id) {
        clearInterval(interval);
        this.loadMyTours(user.Id);
      }
    }, 50);
  }

  goToCreateTour(): void {
    this.router.navigate(['/tours/create']);
  }

  goToAllTours(): void {
    this.router.navigate(['/tours/all']);
  }

  hasSignedIn() {
    return !!this.userService.currentUser;
  }

  loadMyTours(authorId: number): void {
    this.tourService.getMyTours(authorId).subscribe({
      next: (tours) => {
        this.tours = tours;
      },
      error: (err) => {
        console.error('Error loading my tours:', err);
      }
    });
  }

  selectTour(tour: TourResponse): void {
    this.selectedTour = tour;

    this.mapKeyPoints =
      tour.keyPoints?.map((kp: any) => ({
        id: kp.id,
        lat: kp.latitude,
        lng: kp.longitude,
        name: kp.name,
        description: kp.description,
        image: kp.imageUrl
      })) || [];
  }
}