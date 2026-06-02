import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SharedModule } from '../../shared/shared.module';
import { TourService } from '../../services/tour.service';
import { KeyPointService } from '../../services/key-point.service';
import { KeyPoint as ApiKeyPoint } from '../../models/key-point.model';
import { KeyPoint as MapKeyPoint } from '../../shared/map/map.component';
import { UserService } from '../../services/user.service';
import { Router } from '@angular/router';
import { TourReviewsComponent } from '../tour-reviews/tour-reviews.component';
import { PurchaseService } from '../../services/purchase.service';

export interface Tour {
  id: string;
  name: string;
  description: string;
  difficulty: string;
  tags: string[];
  keyPoints: ApiKeyPoint[];
  durations: { transportType: number | string; durationInMinutes: number }[];
  lengthInKm: number;
  price: number;
  status: string;
  isPurchased: boolean;
}

@Component({
  selector: 'app-all-tours',
  standalone: true,
  imports: [CommonModule, FormsModule, SharedModule, TourReviewsComponent],
  templateUrl: './all-tours.html',
  styleUrls: ['./all-tours.css'],
})
export class AllToursComponent implements OnInit {
  tours: Tour[] = [];
  selectedTour: Tour | null = null;
  mapKeyPoints: MapKeyPoint[] = [];
  mapMode: 'create' | 'view' | 'edit' = 'view';

  isEditingKeyPoint = false;
  isFormVisible = false;
  currentPoint: MapKeyPoint = { lat: 0, lng: 0, name: '', description: '', image: '' };
  editingKeyPointId: string | null = null;
  tourLength = 0;
  purchaseMessage = '';
  purchaseError = '';

  constructor(
    private tourService: TourService,
    private keyPointService: KeyPointService,
    private purchaseService: PurchaseService,
    public userService: UserService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadTours();
  }

  goToCreateTour(): void {
    this.router.navigate(['/tours/create']);
  }

  goToMyTours(): void {
    this.router.navigate(['/tours/my']);
  }

  hasSignedIn() {
    return !!this.userService.currentUser;
  }

  isTourist(): boolean {
    return this.userService.currentUser?.role === 'TOURIST';
  }

  isPublished(tour: Tour): boolean {
    return String(tour.status).toLowerCase() === 'published';
  }

  transportLabel(transportType: number | string): string {
    const labels: Record<number, string> = {
      0: 'Walking',
      1: 'Bicycle',
      2: 'Car'
    };

    if (typeof transportType === 'number') {
      return labels[transportType] || `Transport ${transportType}`;
    }

    return transportType;
  }

  loadTours(): void {
    this.tourService.getAllTours().subscribe((tours: Tour[]) => {
      this.tours = tours;
    });
  }

  selectTour(tour: Tour): void {
    this.purchaseMessage = '';
    this.purchaseError = '';
    this.isEditingKeyPoint = false;
    this.isFormVisible = false;
    this.mapMode = 'view';

    this.tourService.getTourById(tour.id).subscribe((freshTour: Tour) => {
      this.setSelectedTour(freshTour);
    });
  }

  setSelectedTour(tour: Tour): void {
    this.selectedTour = tour;
    this.tourLength = tour.lengthInKm;
    this.mapKeyPoints = (tour.keyPoints || []).map(kp => ({
      id: kp.id,
      lat: kp.latitude,
      lng: kp.longitude,
      name: kp.name,
      description: kp.description,
      image: kp.imageUrl,
    }));
  }

  addToCart(tour: Tour): void {
    this.purchaseMessage = '';
    this.purchaseError = '';
    this.purchaseService.addToCart(tour.id).subscribe({
      next: () => {
        this.purchaseMessage = 'Tour added to cart.';
      },
      error: err => {
        this.purchaseError = err?.error?.error || 'Failed to add tour to cart.';
      }
    });
  }

  startEditKeyPoint(kp: MapKeyPoint): void {
    this.isEditingKeyPoint = true;
    this.isFormVisible = true;
    this.mapMode = 'edit';
    this.editingKeyPointId = kp.id!;
    this.currentPoint = { ...kp };
  }

  startAddNewKeyPoint(): void {
    this.isFormVisible = true;
    this.isEditingKeyPoint = false;
    this.mapMode = 'edit';
    this.currentPoint = { lat: 0, lng: 0, name: '', description: '', image: '' };
    this.editingKeyPointId = null;
  }

  deleteKeyPoint(kp: MapKeyPoint): void {
    if (!this.selectedTour || !kp.id) return;
    if (confirm(`Are you sure you want to delete key point: ${kp.name}?`)) {
      this.keyPointService.deleteKeyPoint(this.selectedTour.id, kp.id).subscribe(() => {
        this.selectedTour!.keyPoints = this.selectedTour!.keyPoints.filter(p => p.id !== kp.id);
        this.mapKeyPoints = this.mapKeyPoints.filter(p => p.id !== kp.id);
      });
    }
  }

  handlePointSelected(coords: MapKeyPoint): void {
    if (this.mapMode !== 'edit') return;

    this.currentPoint.lat = coords.lat;
    this.currentPoint.lng = coords.lng;
  }

  handlePointMoved(event: { index: number, lat: number, lng: number }): void {
    if (this.mapMode !== 'edit') return;

    const movedMarker = this.mapKeyPoints[event.index];
    if (this.isEditingKeyPoint && movedMarker.id === this.editingKeyPointId) {
      this.currentPoint.lat = event.lat;
      this.currentPoint.lng = event.lng;
    }

    this.mapKeyPoints = this.mapKeyPoints.map((p, idx) => {
      if (idx !== event.index) return p;
      return {
        ...p,
        lat: event.lat,
        lng: event.lng
      };
    });
  }

  saveKeyPoint(): void {
    if (!this.selectedTour || !this.currentPoint.name) {
      alert('Key point name is required.');
      return;
    }

    const payload = {
      name: this.currentPoint.name || '',
      description: this.currentPoint.description || '',
      imageUrl: this.currentPoint.image || '',
      latitude: this.currentPoint.lat,
      longitude: this.currentPoint.lng,
    };

    if (this.isEditingKeyPoint && this.editingKeyPointId) {
      this.keyPointService.updateKeyPoint(this.selectedTour.id, this.editingKeyPointId, payload)
        .subscribe(updatedKp => {
          const apiIndex = this.selectedTour!.keyPoints.findIndex(p => p.id === this.editingKeyPointId);
          if (apiIndex > -1) this.selectedTour!.keyPoints[apiIndex] = updatedKp;

          this.mapKeyPoints = this.mapKeyPoints.map(p => {
            if (p.id === this.editingKeyPointId) {
              return {
                id: updatedKp.id,
                lat: this.currentPoint.lat,
                lng: this.currentPoint.lng,
                name: this.currentPoint.name,
                description: this.currentPoint.description,
                image: this.currentPoint.image
              };
            }
            return p;
          });

          this.resetForm();
        });
    } else {
      this.keyPointService.addKeyPoint(this.selectedTour.id, payload)
        .subscribe(newKp => {
          this.selectedTour?.keyPoints.push(newKp);

          this.mapKeyPoints = [...this.mapKeyPoints, {
            id: newKp.id,
            lat: newKp.latitude,
            lng: newKp.longitude,
            name: newKp.name,
            description: newKp.description,
            image: newKp.imageUrl
          }];
          this.resetForm();
        });
    }
  }

  cancelEdit(): void {
    this.resetForm();
  }

  private resetForm(): void {
    this.isEditingKeyPoint = false;
    this.isFormVisible = false;
    this.mapMode = 'view';
    this.editingKeyPointId = null;
    this.currentPoint = { lat: 0, lng: 0, name: '', description: '', image: '' };
  }

  onTourLengthChanged(length: number) {
    this.tourLength = length;

    if (this.selectedTour) {
      this.selectedTour.lengthInKm = length;

      this.tourService
        .updateTourLength(this.selectedTour.id, length)
        .subscribe();
    }
  }

  startTour(tourId: string): void {
    const activeExecId = localStorage.getItem('activeExecutionId');
    const activeTourId = localStorage.getItem('activeTourId');
    
    if (activeExecId && activeTourId === tourId) {
      this.router.navigate(['/tours/active'], { 
        queryParams: { tourId, executionId: activeExecId } 
      });
    } else {
      this.router.navigate(['/tours/active'], { queryParams: { tourId } });
    }
  }
}
