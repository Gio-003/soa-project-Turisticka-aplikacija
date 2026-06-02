import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SharedModule } from '../../shared/shared.module';
import { ActivatedRoute, Router } from '@angular/router';
import { TourService } from '../../services/tour.service';
import { UserService } from '../../services/user.service';
import { KeyPoint } from '../../shared/map/map.component';

@Component({
  selector: 'app-active-tour',
  standalone: true,
  imports: [CommonModule, FormsModule, SharedModule],
  templateUrl: './active-tour.component.html',
  styleUrls: ['./active-tour.component.css']
})
export class ActiveTourComponent implements OnInit, OnDestroy {
  tour: any = null;
  execution: any = null;
  executionId: string | null = null;
  mapPoints: KeyPoint[] = [];
  touristMarker: KeyPoint | null = null;
  allMapPoints: KeyPoint[] = [];
  statusMessage = '';
  isLoading = true;
  private intervalId: any;
  private simulatedLat: number = 0;
  private simulatedLng: number = 0;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private tourService: TourService,
    private userService: UserService
  ) {}

  ngOnInit(): void {
    const tourId = this.route.snapshot.queryParamMap.get('tourId');
    if (!tourId) {
      this.router.navigate(['/tours/all']);
      return;
    }

    const touristId = this.userService.currentUser?.id;
    if (!touristId) {
      this.router.navigate(['/login']);
      return;
    }

    const existingExecutionId = this.route.snapshot.queryParamMap.get('executionId');

    this.tourService.getTourById(tourId).subscribe({
      next: (tour: any) => {
        this.tour = tour;
        this.mapPoints = (tour.keyPoints || []).map((kp: any) => ({
          id: kp.id,
          lat: kp.latitude,
          lng: kp.longitude,
          name: kp.name,
          description: kp.description
        }));

        if (existingExecutionId) {
          this.executionId = existingExecutionId;
          this.tourService.getExecution(existingExecutionId).subscribe({
            next: (exec: any) => {
              this.execution = exec;
              this.initPosition();
              this.isLoading = false;
              this.startActivityInterval();
            },
            error: () => {
              this.statusMessage = 'Failed to load existing execution.';
              this.isLoading = false;
            }
          });
        } else {
          this.tourService.startExecution(
            tourId, touristId,
            this.mapPoints[0]?.lat ?? 45.2396,
            this.mapPoints[0]?.lng ?? 19.8227
          ).subscribe({
            next: (exec: any) => {
              this.execution = exec;
              this.executionId = exec.id;
              localStorage.setItem('activeExecutionId', exec.id);
              localStorage.setItem('activeTourId', tourId);
              this.initPosition();
              this.isLoading = false;
              this.startActivityInterval();
            },
            error: (err: any) => {
              this.statusMessage = err?.error?.error || 'Failed to start tour execution.';
              this.isLoading = false;
            }
          });
        }
      },
      error: () => {
        this.statusMessage = 'Failed to load tour.';
        this.isLoading = false;
      }
    });
  }

  private initPosition(): void {
    if (this.mapPoints.length === 0) return;
    this.simulatedLat = this.mapPoints[0].lat;
    this.simulatedLng = this.mapPoints[0].lng;
    this.updateTouristMarker();
  }

  private startActivityInterval(): void {
    this.intervalId = setInterval(() => {
      if (!this.executionId) return;
      this.tourService.checkPosition(
        this.executionId,
        this.simulatedLat,
        this.simulatedLng
      ).subscribe({
        next: (result: any) => {
          this.execution.lastActivity = result.lastActivity;
          if (result.keyPointCompleted && result.nearbyKeyPointName) {
            this.statusMessage = `✅ Reached: ${result.nearbyKeyPointName}`;
            this.execution?.completedKeyPoints?.push({
              keyPointId: result.nearbyKeyPointId
            });
            this.updateTouristMarker();
          }
        }
      });
    }, 10000);
  }

  onMapClick(point: KeyPoint): void {
    this.simulatedLat = point.lat;
    this.simulatedLng = point.lng;
    this.updateTouristMarker();

    if (!this.executionId) return;
    this.tourService.checkPosition(
      this.executionId, point.lat, point.lng
    ).subscribe({
      next: (result: any) => {
        this.execution.lastActivity = result.lastActivity;
        if (result.keyPointCompleted && result.nearbyKeyPointName) {
          this.statusMessage = `✅ Reached: ${result.nearbyKeyPointName}`;
          this.execution?.completedKeyPoints?.push({
            keyPointId: result.nearbyKeyPointId
          });
          this.updateTouristMarker();
        } else {
          this.statusMessage = 'No key point nearby.';
        }
      }
    });
  }

  private updateTouristMarker(): void {
    this.touristMarker = {
      lat: this.simulatedLat,
      lng: this.simulatedLng,
      name: '📍 You are here'
    };

    const uncompletedPoints = this.mapPoints.filter(
      kp => !this.isKeyPointCompleted(kp.id || '')
    );

    this.allMapPoints = [...uncompletedPoints, this.touristMarker];
  }

  isKeyPointCompleted(keyPointId: string): boolean {
    return (this.execution?.completedKeyPoints || [])
      .some((ckp: any) => ckp.keyPointId === keyPointId);
  }

  completeTour(): void {
    if (!this.executionId) return;
    this.tourService.completeExecution(this.executionId).subscribe(() => {
      this.clearAndNavigate('Tour completed! 🎉');
    });
  }

  abandonTour(): void {
    if (!this.executionId) return;
    this.tourService.abandonExecution(this.executionId).subscribe(() => {
      this.clearAndNavigate('Tour abandoned.');
    });
  }

  private clearAndNavigate(msg: string): void {
    clearInterval(this.intervalId);
    localStorage.removeItem('activeExecutionId');
    localStorage.removeItem('activeTourId');
    alert(msg);
    this.router.navigate(['/tours/all']);
  }

  ngOnDestroy(): void {
    clearInterval(this.intervalId);
  }
}