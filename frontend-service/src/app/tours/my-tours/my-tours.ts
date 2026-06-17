import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TourService } from '../../services/tour.service';
import { UserService } from '../../services/user.service';
import { SharedModule } from '../../shared/shared.module';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { TourDurationService } from '../../services/tour-duration.service';

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
  durations?: {
    transportType: 'Walking' | 'Bicycle' | 'Car';
    durationInMinutes: number;
  }[];
}

@Component({
  selector: 'app-my-tours',
  standalone: true,
  imports: [CommonModule, SharedModule, FormsModule],
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
    private router: Router,
    private tourDurationService: TourDurationService
  ) { }

  ngOnInit(): void {
    const user = this.userService.currentUser;

    if (user?.id) {
      this.loadMyTours(user.id);
    }
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

  getTransportIcon(type: string): string {
    switch (type) {
      case 'Walking': return '🚶';
      case 'Bicycle': return '🚴';
      case 'Car': return '🚗';
      default: return '📍';
    }
  }

  hasTourDurations(tour: TourResponse): boolean {
    return tour.durations?.length == 3;
  }

  selectTour(tour: TourResponse): void {

    // RESET DURATION FORME
    this.durations = [];

    this.newDuration = {
      transportType: 'Walking',
      durationInMinutes: 0
    };

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

  // =====================================================
  // ✅ NOVO: TOUR DURATIONS (LOCAL EDIT STATE)
  // =====================================================

  durations: {
    transportType: 'Walking' | 'Bicycle' | 'Car';
    durationInMinutes: number;
  }[] = [];   // NOVO

  newDuration = {
    transportType: 'Walking' as 'Walking' | 'Bicycle' | 'Car',
    durationInMinutes: 0
  }; // NOVO

  addDuration(): void {

    const minutes = Number(this.newDuration.durationInMinutes);

    if (!minutes || minutes <= 0) {
      alert('Unesite validno vreme');
      return;
    }

    const transportType = this.newDuration.transportType;

    // Provera u već sačuvanim trajanjima ture
    const existsInTour =
      this.selectedTour?.durations?.some(
        d => d.transportType === transportType
      ) ?? false;

    // Provera u trenutno dodatim (a još nesačuvanim) trajanjima
    const existsInCurrent =
      this.durations.some(
        d => d.transportType === transportType
      );

    if (existsInTour || existsInCurrent) {
      alert(`Trajanje za ${transportType} već postoji.`);
      return;
    }

    this.durations = [
      ...this.durations,
      {
        transportType,
        durationInMinutes: minutes
      }
    ];

    this.newDuration = {
      transportType: this.newDuration.transportType,
      durationInMinutes: 0
    };
  }

  removeDuration(index: number): void { // NOVO
    this.durations = this.durations.filter((_, i) => i !== index);
  }

  saveDuration(): void {

    if (!this.selectedTour) {
      return;
    }

    for (const duration of this.durations) {
      this.tourDurationService
        .addDuration(this.selectedTour.id, duration)
        .subscribe();
    }

    this.selectedTour.durations = [
      ...(this.selectedTour.durations ?? []),
      ...this.durations
    ];

    this.durations = [];

    this.newDuration = {
      transportType: 'Walking',
      durationInMinutes: 0
    };
  }

  publishTour(id: string): void {

    if (!this.selectedTour?.keyPoints || this.selectedTour.keyPoints.length === 0) {
      alert('Morate dodati key points pre objavljivanja ture.');
      return;
    }

    if (!this.selectedTour?.durations || this.selectedTour.durations.length === 0) {
      alert('Potrebno je da popunite sve podatke pre objavljivanja ture.');
      return;
    }

    this.tourService.publishTour(id).subscribe({
      next: () => {
        const tour = this.tours.find(t => t.id === id);
        if (tour) {
          tour.status = 'Published';
        }
      },
      error: (err) => {
        console.error('Error publishing tour:', err);
      }
    });
  }

  archiveTour(id: string): void {
    this.tourService.archiveTour(id).subscribe({
      next: () => {
        const tour = this.tours.find(t => t.id === id);
        if (tour) {
          tour.status = 'Archived';
        }
      },
      error: (err) => {
        console.error('Error archiving tour:', err);
      }
    });
  }
}