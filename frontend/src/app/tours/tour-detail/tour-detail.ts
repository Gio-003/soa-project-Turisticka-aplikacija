import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { TourService } from '../../services/tour.service';
import { ReviewListComponent } from '../review-list/review-list';
import { CreateReviewComponent } from '../create-review/create-review';

interface Tour {
  id: string;
  name: string;
  description: string;
  difficulty: string;
  price: number;
  status: string;
  authorId: string;
  tags: string[];
}

@Component({
  selector: 'app-tour-detail',
  standalone: true,
  imports: [CommonModule, ReviewListComponent, CreateReviewComponent],
  templateUrl: './tour-detail.html',
  styleUrls: ['./tour-detail.css']
})
export class TourDetailComponent implements OnInit {
  tour: Tour | null = null;
  tourId: string = '';
  loading = true;
  showReviewForm = false;

  constructor(
    private route: ActivatedRoute,
    private tourService: TourService
  ) {}

  ngOnInit(): void {
    this.tourId = this.route.snapshot.paramMap.get('tourId') || '';
    if (this.tourId) {
      this.loadTour();
    }
  }

  loadTour(): void {
    this.tourService.getTourById(this.tourId).subscribe({
      next: (data) => {
        this.tour = data;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading tour:', err);
        this.loading = false;
      }
    });
  }

  getDifficultyColor(difficulty: string): string {
    switch (difficulty?.toLowerCase()) {
      case 'lako':
        return '#28a745';
      case 'srednje':
        return '#ffc107';
      case 'teško':
        return '#dc3545';
      default:
        return '#6c757d';
    }
  }
}
